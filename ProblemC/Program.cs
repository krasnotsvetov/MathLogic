using LogicParser;
using LogicParser.Operators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemC
{
    public class Program
    {
        static void Main(string[] args)
        {
            new Solver().Run();
        }

        public class Solver
        {

            static List<Axiom> axioms;
            private static string[] aImplA;
            private static string[] mpTransform;

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


            private bool CheckGenerality(Expression expression, out Dictionary<Constant, bool> constants)
            {
                constants = new Dictionary<Constant, bool>();
                var allConstants = Evaluator.GetAllVariables(expression);

                foreach (var c in allConstants)
                {
                    constants[c] = false;
                }

                for (int i = 0; i < Math.Pow(2, allConstants.Count); i++)
                {
                    for (int j = 0; j < allConstants.Count; j++)
                    {
                        constants[allConstants[j]] = (i & (1 << j)) == 0 ? false : true;
                    }
                    if (!Evaluator.Evaluate(expression, constants))
                    {
                        return false;
                    }
                }
                return true;
            }

            public void Run()
            {
                
                Parser parser = new Parser();
                Expression expr;
                using (var sw = new StreamReader(new FileStream("test.in", FileMode.Open)))
                {
                    expr = parser.Parse(sw.ReadLine());
                }
                if (!CheckGenerality(expr, out var badCombination))
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream("test.out", FileMode.Create)))
                    {
                        sw.Write("Высказывание ложно при : ");
                        foreach (var t in badCombination)
                        {
                            sw.Write(t.Key + ":" + (t.Value == true ? "И " : "Л "));
                        }
                        sw.WriteLine();
                    }
                    return;
                }

                var ans = Proof(expr);
                using (StreamWriter sw = new StreamWriter(new FileStream("test.out", FileMode.Create)))
                {
                    sw.WriteLine($"|-{expr.ToString()}");
                    foreach (var l in ans)
                    {
                        sw.WriteLine(l.ToString());
                    }
                }
            }


            public List<Expression> Proof(Expression expression)
            {
                List<Dictionary<Constant, bool>> values = new List<Dictionary<Constant, bool>>();
                var allConstants = Evaluator.GetAllVariables(expression);

                for (int i = 0; i < Math.Pow(2, allConstants.Count); i++)
                {
                    values.Add(new Dictionary<Constant, bool>());
                    for (int j = allConstants.Count - 1; j >= 0 ; j--)
                    {
                        values[i][allConstants[j]] = (i & (1 << j)) == 0 ? false : true;
                    } 
                }

                List<List<Expression>> proofs = new List<List<Expression>>();

                for (int i = 0; i < Math.Pow(2, allConstants.Count); i++)
                {
                    proofs.Add(DeepProof(expression, values[i]));
                }
                int size = proofs.Count;
                int level = 0;
                while (size != 1)
                {
                    var mergedProof = new List<List<Expression>> ();
                    var mergedValues = new List<Dictionary<Constant, bool>>();
                    for (int i = 0; i < size; i += 2)
                    {
                        mergedValues.Add(values[i]);
                        mergedProof.Add(Merge(expression, proofs[i], proofs[i + 1], values[i], values[i + 1], level));
                    } 
                    proofs = mergedProof;
                    values = mergedValues;
                    level++;
                    size /= 2;
                }

                return proofs[0];
            }

            private List<Expression> Merge(Expression expr, List<Expression> first, List<Expression> second, Dictionary<Constant, bool> valuesFirst, Dictionary<Constant, bool> valuesSecond, int level)
            {
                var keys = valuesFirst.Keys.ToList();
                keys.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));
                List<Expression> assumptionA = new List<Expression>();
                List<Expression> assumptionB = new List<Expression>();
                for (int i = 0; i < keys.Count - level; i++)
                {
                    if (valuesFirst[keys[i]])
                    {
                        assumptionA.Add(keys[i]);
                    } else
                    {
                        assumptionA.Add(new Negation(keys[i]));
                    }

                    if (valuesSecond[keys[i]])
                    {
                        assumptionB.Add(keys[i]);
                    }
                    else
                    {
                        assumptionB.Add(new Negation(keys[i]));
                    }
                }

                first = Deduction(first, assumptionA);
                second = Deduction(second, assumptionB);
                var aOrNotA = ProofCollection.GetAOrNotA();
                for (int i = 0; i < aOrNotA.Count; i++)
                {
                    aOrNotA[i] = SubstituteExprsToExpr(aOrNotA[i], assumptionB.Last(), null);
                }

                var parser = new Parser();

                var axiom8 = SubstituteExprsToExpr(parser.Parse("((A->B)->(!A->B)->(A|!A)->B)"), assumptionB.Last(), expr);
                var mp1 = SubstituteExprsToExpr(parser.Parse("((!A->B)->(A|!A)->B)"), assumptionB.Last(), expr);

                var mp2 = SubstituteExprsToExpr(parser.Parse("((A|!A)->B)"), assumptionB.Last(), expr);


                var rv = new List<Expression>();
                rv.AddRange(first);
                rv.AddRange(second);
                rv.AddRange(aOrNotA);
                rv.Add(axiom8);
                rv.Add(mp1);
                rv.Add(mp2);

                rv.Add(expr.Clone());
                return rv;
            }

            private List<Expression> DeepProof(Expression expression, Dictionary<Constant, bool> values)
            {
                switch (expression)
                {
                    case Constant c:
                        var value = Evaluator.Evaluate(c, values);
                        var proof = ProofCollection.GetProof(typeof(Negation), value);
                        for (int i = 0; i < proof.Count; i++)
                        {
                            proof[i] = SubstituteExprsToExpr(proof[i], c, null);
                        }
                        return proof;
                    case UnaryOperation uo:
                        proof = new List<Expression>();
                        value = Evaluator.Evaluate(uo.Expression, values);

                        if (!(uo.Expression is Constant))
                        {
                            proof.AddRange(DeepProof(uo.Expression, values));
                        }

                        var tempProof = ProofCollection.GetProof(uo.GetType(), value);
                        for (int i = 0; i < tempProof.Count; i++)
                        { 
                            tempProof[i] = SubstituteExprsToExpr(tempProof[i], uo.Expression, null);
                        }
                        proof.AddRange(tempProof);
                        return proof;
                    case BinaryOperation bo:
                        proof = new List<Expression>();
                        var leftValue = Evaluator.Evaluate(bo.Left, values);
                        var rightValue = Evaluator.Evaluate(bo.Right, values);
                        proof.AddRange(DeepProof(bo.Left, values));
                        proof.AddRange(DeepProof(bo.Right, values));
                        tempProof = ProofCollection.GetProof(bo.GetType(), leftValue , rightValue);
                        for (int i = 0; i < tempProof.Count; i++)
                        {
                            
                            tempProof[i] = SubstituteExprsToExpr(tempProof[i], bo.Left, bo.Right);
                        }
                        proof.AddRange(tempProof);
                        return proof;

                }
                throw new Exception();
            }


            private Expression SubstituteExprsToExpr(Expression baseExpression, Expression A, Expression B)
            {

                switch (baseExpression)
                {
                    case Constant constant:
                        return constant.Equals("A") ? A : B;
                    case BinaryOperation bo:
                        if (bo.Left is Constant c)
                        {
                            bo.Left = c.Value.Equals("A") ? A : B;
                        }
                        else
                        {
                            SubstituteExprsToExpr(bo.Left, A, B);
                        }

                        if (bo.Right is Constant c2)
                        {
                            bo.Right = c2.Value.Equals("A") ? A : B;
                        }
                        else
                        {
                            SubstituteExprsToExpr(bo.Right, A, B);
                        }

                        break;
                    case UnaryOperation uo:
                        if (uo.Expression is Constant c3)
                        {
                            uo.Expression = c3.Value.Equals("A") ?  A : B;
                        }
                        else
                        {
                            SubstituteExprsToExpr(uo.Expression, A, B);
                        }
                        break;
                }
                return baseExpression;
            }
             
            private List<Expression> Deduction(List<Expression> proof, List<Expression> assumptions)
            {
                var newProof = new List<Expression>();
                var parser = new Parser();
                for (int l = 0; l < proof.Count; l++)
                {
                    bool needContinue = false;
                    var expr = proof[l];
                    if (expr.Equals(assumptions.Last()))
                    {
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
                return newProof;
            }
        }
    }
}
