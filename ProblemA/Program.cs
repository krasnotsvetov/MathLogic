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

namespace ProblemA
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
            private List<Expression> proof = new List<Expression>();
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
                    new Axiom("!!a->a")
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
                Console.WriteLine($"Debug: checking...");
#endif
                for (int l  = 0; l < proof.Count; l++)
                {
                    var expr = proof[l];
                    foreach (var axiom in axioms)
                    {
                        if (axiom.IsMatch(expr))
                        {
                            expr.Tag = $"Сх. акс. {axioms.IndexOf(axiom) + 1}";
                            break;
                        }
                    }

                    if (expr.Tag != null) continue;

                    foreach (var a in assumptions)
                    {
                        if (a.Equals(expr))
                        {
                            expr.Tag = $"Предп. {assumptions.IndexOf(a) + 1}";
                            break;
                        }
                    }

                    if (expr.Tag != null) continue;

                    for (int j = l - 1; j >= 0; j--)
                    {
                        bool needBreak = false;
                        if (proof[j] is Implication impl && impl.Right.Equals(expr))
                        {
                            for (int i = l - 1; i >= 0; i--)
                            {
                                if (impl.Left.Equals(proof[i]))
                                {
                                    expr.Tag = $"M.P. {i + 1}, {j + 1}";
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

                    if (expr.Tag == null)
                    {
#if DEBUG
                        Debug.WriteLine($"Warning! Can't proof line {proof.IndexOf(expr) + 1}");
#endif
                        expr.Tag = "Не доказано";
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
                    for (int i = 0; i < proof.Count; i++)
                    {
                        sw.WriteLine($"({i + 1}) {proof[i]} ({proof[i].Tag})");
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
