using Common.Extension;
using LogicParser;
using LogicParser.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemB
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Solver().Run();
        }

        public class Solver
        {
            private static List<Axiom> axioms;
            private static string[] aImplA;
            private static string[] mpTransform;
            private List<Expression> proof = new List<Expression>();
            private List<Expression> newProof = new List<Expression>();
            private List<Expression> assumptions = new List<Expression>();

            static Solver()
            {
                axioms = new List<Axiom>()
                {
                    new Axiom("a->b->a"),
                    new Axiom("(a->b)->(a->b->c)->(a->c)"),
                    new Axiom("a->b->a&b"),
                    new Axiom("a&b->a"),
                    new Axiom("a&b->b"),
                    new Axiom("a->a|b"),
                    new Axiom("b->a|b"),
                    new Axiom("(a->b)->(c->b)->(a|c->b)"),
                    new Axiom("(a->b)->(a->!b)->!a"),
                    new Axiom("!!a->b")
                };

                aImplA = new string[] {
                     "(a->(a->a))",
                     "(a->(a->a))->(a->((a->a)->a))->(a->a)",
                     "(a->((a->a)->a))->(a->a)",
                     "(a->((a->a)->a))",
                     "(a->a)"
                };

                mpTransform = new string[] {
                    "(a->bj)->((a->(bj->bi))->(a->bi))",
                    "((a->(bj->bi))->(a->bi)",
                    "(a->bi)"
                };
            }

            public Solver()
            {

            }

            private readonly string InputFile = "test.in";
            private readonly string OutputFile = "test.out";
            private string header;

            public void Run()
            {
#if DEBUG
                var watch = new Stopwatch();
                float totalTime = 0;
                Console.WriteLine("Debug : reading & parsing start");
                watch.Start();
#endif

                Parser parser = new Parser();

                using (var sr = new StreamReader(new FileStream(InputFile, FileMode.Open)))
                {
                    string line = header = sr.ReadLine();
                    string[] firstLine = line.Split(new string[] { "|-" }, StringSplitOptions.None);
                    foreach (var str in firstLine[0].Split(','))
                    {
                        assumptions.Add(parser.Parse(str));
                    }
                    while ((line = sr.ReadLine()) != null)
                    {
                        proof.Add(parser.Parse(line));
                    }
                }
#if DEBUG
                watch.Stop();
                Console.WriteLine($"Debug: reading & parsing end. Total time : {watch.ElapsedMilliseconds} ms");
                totalTime += watch.ElapsedMilliseconds;
                watch.Reset();
                watch.Start();
                Console.WriteLine($"Debug: transform...");
#endif
                for (int l = 0; l < proof.Count; l++)
                {
                    bool needContinue = false;
                    var expr = proof[l];
                    if (expr.Equals(assumptions.Last())) {
                        string t = expr.ToString();
                        for (int i = 0; i < aImplA.Length; i++)
                        {
                            var newExpr = aImplA[i].Clone() as string;
                            newProof.Add(parser.Parse(newExpr.Replace("a", t)));
                        }
                        needContinue = true;
                    }

                    if (needContinue) continue;

                    foreach (var axiom in axioms)
                    {
                        if (axiom.IsMatch(expr))
                        {
                            newProof.Add(expr);
                            newProof.Add(new Implication(expr, new Implication(assumptions.Last(), expr)));
                            newProof.Add(new Implication(assumptions.Last(), expr));
                            needContinue = true;
                            break;
                        }
                    }

                    if (needContinue) continue;

                    //TODO: fix copy-paste
                    foreach (var a in assumptions)
                    {
                        if (a.Equals(expr))
                        {
                            newProof.Add(expr);
                            newProof.Add(new Implication(expr, new Implication(assumptions.Last(), expr)));
                            newProof.Add(new Implication(assumptions.Last(), expr));
                            needContinue = true;
                            break;
                        }
                    }

                    if (needContinue) continue;

                    for (int j = l - 1; j >= 0; j--)
                    {
                        bool needBreak = false;
                        if (proof[j] is Implication impl && impl.Right.Equals(expr))
                        {
                            for (int i = l - 1; i >= 0; i--)
                            {
                                if (impl.Left.Equals(proof[i]))
                                {
                                    
                                    for (int z = 0; z < mpTransform.Length; z++)
                                    {
                                        var t = mpTransform[z].Clone() as string;
                                        t = t.Replace("a", assumptions.Last().ToString());
                                        t = t.Replace("bj", impl.Left.ToString());
                                        t = t.Replace("bi", expr.ToString());
                                        newProof.Add(parser.Parse(t));
                                    }
                                    needContinue = true;
                                    needBreak = true;
                                    break;
                                }
                            }
                        }
                        if (needBreak)
                        {
                            break;
                        }
                    }

                    if (!needContinue)
                    {
                        throw new Exception();
                    }
                }

#if DEBUG
                watch.Stop();
                Console.WriteLine($"Debug: checking. Total time : {watch.ElapsedMilliseconds} ms");
                totalTime += watch.ElapsedMilliseconds;
                watch.Reset();
                watch.Start();
                Console.WriteLine($"Debug: printing...");
#endif
                using (StreamWriter sw = new StreamWriter(new FileStream(OutputFile, FileMode.Create)))
                {
                    sw.WriteLine(header);
                    for (int i = 0; i < newProof.Count; i++)
                    {
                        sw.WriteLine($"{newProof[i]}");
                    }
                }

#if DEBUG
                watch.Stop();
                Console.WriteLine($"Debug: printing. Total time : {watch.ElapsedMilliseconds} ms");
                totalTime += watch.ElapsedMilliseconds;
                Console.WriteLine($"Debug: total time : {totalTime} ms");
#endif
            }
        }
    }
}
