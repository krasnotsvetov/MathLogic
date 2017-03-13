using PredicateParser;
using PredicateParser.Operators;
using PredicateParser.Operators.Logic;
using PredicateParser.Operators.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemD
{
    class Program
    {
        static void Main(string[] args)
        {
            new Solver().Run();
        }

        public class Solver
        {
            List<AxiomScheme> axiomSchemes = new List<AxiomScheme>();
            List<IExpression> axioms = new List<IExpression>();
            List<IExpression> assumptions = new List<IExpression>();
            List<IExpression> proof = new List<IExpression>();
            List<IExpression> newProof = new List<IExpression>();
            List<IExpression> selfCons = new List<IExpression>();

            AxiomScheme inductionAxiom;
            AxiomScheme existenceAxiom;
            AxiomScheme universalAxiom;
            public Solver()
            {
                var p = new Parser();
                 
                

                inductionAxiom = new AxiomScheme("F(0)&@x(F(x)->F(x'))->F(x)");
                existenceAxiom = new AxiomScheme("F(o)->?xF(x)");
                universalAxiom = new AxiomScheme("@xF(x)->F(o)");
                 

                axiomSchemes = new List<AxiomScheme>()
                {
                    new AxiomScheme("A->B->A"),
                    new AxiomScheme("(A->B)->(A->B->C)->(A->C)"),
                    new AxiomScheme("A->B->A&B"),
                    new AxiomScheme("A&B->A"),
                    new AxiomScheme("A&B->B"),
                    new AxiomScheme("A->A|B"),
                    new AxiomScheme("B->A|B"),
                    new AxiomScheme("(A->B)->(C->B)->(A|C->B)"),
                    new AxiomScheme("(A->B)->(A->!B)->!A"),
                    new AxiomScheme("!!A->A")
                };

                axioms = new List<IExpression>()
                {
                    p.Parse("a=b->a'=b'"),
                    p.Parse("a=b->a=c->b=c"),
                    p.Parse("a'=b'->a=b"),
                    p.Parse("!a'=0"),
                    p.Parse("a+b'=(a+b)'"),
                    p.Parse("a+0=a"),
                    p.Parse("a*0=a"),
                    p.Parse("a*b'=a*b+a")
                };

                selfCons = new List<IExpression>()
                {
                    p.Parse("(A->A->A)"),
                    p.Parse("(A->A->A)->(A->(A->A)->A)->(A->A)"),
                    p.Parse("(A->(A->A)->A)->(A->A)"),
                    p.Parse("(A->(A->A)->A)"),
                    p.Parse("A->A")
                };
            }


            private IEnumerable<string> CommaSplit(string s)
            {
                List<string> rv = new List<string>();
                int balans = 0;
                int prev = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '(')
                    {
                        balans++;
                    }
                    if (s[i] == ')')
                    {
                        balans--;
                    }
                    if (balans == 0 && s[i] == ',')
                    {
                        rv.Add(s.Substring(prev, i - prev));
                        prev = i + 1;
                    }
                }
                if (s.Length != 0)
                {
                    rv.Add(s.Substring(prev, s.Length - prev));
                }
                return rv;  
            }
            private IExpression SubstituteVariableToExpr(IExpression baseExpression, Variable x, IExpression y)
            {
                if (y == null)
                {
                    return baseExpression.Clone();
                }
                var rv = baseExpression.Clone();
                Dictionary<Variable, int> closing = new Dictionary<Variable, int>();
                rv = _substituteVariableToExpr(rv, (Variable)x.Clone(), y, closing);
                return rv;
            }

            private IExpression _substituteVariableToExpr(IExpression baseExpression, Variable x, IExpression y, Dictionary<Variable, int> closing)
            {
                if (y.Equals(x))
                {
                    return baseExpression;
                }
                switch (baseExpression)
                {
                    case ArityOperation ao:
                        for (int i = 0; i < ao.Arity; i++)
                        {
                            if (ao.Arguments[i] is Variable v && v.Equals(x) && (!closing.ContainsKey(v) || closing[v] == 0))
                            {
                                bool cantSet(IExpression expr)
                                {
                                    bool has = false;
                                    switch (expr)
                                    {
                                        case ArityOperation yao:
                                            foreach (var arg in yao.Arguments)
                                            {
                                                has |= cantSet(arg);
                                            }
                                            break;
                                        case Variable _v:
                                            return closing.ContainsKey(_v) && closing[_v] > 0 && _v.Value != x.Value;
                                    }
                                    return has;
                                }

                                if (cantSet(y))
                                {
                                    throw new Exception();
                                }
                                ao.Arguments[i] = y;
                            } else if (!(ao.Arguments[i] is Variable))
                            {
                                ao.Arguments[i] = _substituteVariableToExpr(ao.Arguments[i], x, y, closing);
                            }
                        }
                        break;
                    case Quantifier q:
                        if (!closing.ContainsKey(q.Variable))
                        {
                            closing[q.Variable] = 0;
                        }
                        closing[q.Variable]++;
                        q.Expression = _substituteVariableToExpr(q.Expression, x, y, closing);
                        closing[q.Variable]--;
                        break;
                }

                return baseExpression;
            }

            private IExpression SubstituteExprsToExpr(IExpression baseExpression, Dictionary<string, IExpression> map)
            {
                var rv = baseExpression.Clone();
                rv = _substituteExprsToExpr(rv, map);
                return rv;
            }

            private IExpression _substituteExprsToExpr(IExpression baseExpression, Dictionary<string, IExpression> map)
            {

                switch (baseExpression)
                {
                    case Predicate predicate:
                        return map[predicate.Name];
                    case Variable constant:
                        return map[constant.Value]; 
                    case ArityOperation ao:
                        for (int i = 0; i < ao.Arity; i++)
                        {
                            var arg = ao.Arguments[i];
                            ao.Arguments[i] = _substituteExprsToExpr(arg, map);
                        }
                        break;
                    case Quantifier q:
                        q.Variable = (Variable)map[q.Variable.Value];
                        q.Expression = _substituteExprsToExpr(q.Expression, map);
                        break;
                }
                return baseExpression;
            }

            private List<IExpression> BaseDeduct(IExpression expr, IExpression alpha)
            {
                var t = SubstituteExprsToExpr(axiomSchemes[0].Expression, new Dictionary<string, IExpression>()
                {
                    {"A", expr },
                    {"B", alpha}
                });

                return new List<IExpression>()
                {
                    t,
                    expr,
                    new Implication(alpha, expr)
                };
            }
            
            private IExpression GetMismatch(IExpression baseExpr, IExpression substExpr, Variable var)
            {
                switch (baseExpr)
                {
                    case Zero zero:
                        if (substExpr is Zero)
                        {
                            return null;
                        }
                        throw new SkipException();
                    case ArityOperation ao:
                        if (substExpr is ArityOperation s && s.Arity == ao.Arity && s.Name.Equals(ao.Name))
                        {
                            for (int i = 0; i < s.Arity; i++)
                            {
                                var ans = GetMismatch(ao.Arguments[i], s.Arguments[i], var);
                                if (ans != null)
                                {
                                    return ans;
                                }
                            }
                            return null;
                        }
                        throw new SkipException();
                    case Quantifier q:
                        if (substExpr is Quantifier sq && q.Function.Equals(sq.Function))
                        {
                            return GetMismatch(q.Expression, sq.Expression, var);
                        }
                        throw new SkipException();
                    case Variable v:
                        if (v.Equals(var))
                        {
                            return substExpr;
                        }
                        return null;
                }
                throw new SkipException();
            }

            public void Run()
            {
                string header;
                IExpression toProof = null;

                Dictionary<IExpression, int> cache = new Dictionary<IExpression, int>();
                var parser = new Parser();
                IExpression alpha = null;
                string reason = "";
                bool isProoveCorrect = true;
                using (var sr = new StreamReader(new FileStream("test.in", FileMode.Open)))
                {
                    string line = header = sr.ReadLine();
                    var temp = line.Split(new string[] { "|-" }, StringSplitOptions.None);
                    toProof = parser.Parse(temp[1]);
                    foreach (var s in  CommaSplit(temp[0]))
                    {
                        assumptions.Add(alpha = parser.Parse(s));
                    }
                    while ((line = sr.ReadLine()) != null)
                    {
                        proof.Add(parser.Parse(line));
                    }
                }

                if (alpha != null)
                {
                    assumptions.Remove(alpha);
                }
                int i = 0;
                for  (i = 0; i < proof.Count; i++)
                {
                    var expr = proof[i];
                    var isProofed = false;
                    #region axioms
                    foreach (var axiom in axioms)
                    {
                        if (expr.Equals(axiom))
                        {
                            isProofed = true;
                            if (alpha != null)
                            {
                                newProof.AddRange(BaseDeduct(expr, alpha));
                            }
                            break;
                        }
                    }
                    #endregion
                    #region assumptions
                    if (!isProofed)
                    foreach (var assump in assumptions)
                    {
                        if (expr.Equals(assump))
                        {
                            isProofed = true;
                            if (alpha != null)
                            {
                                newProof.AddRange(BaseDeduct(expr, alpha));
                            }
                            break;
                        }
                    }
                    #endregion
                    #region axiomSchemes
                    if (!isProofed)
                    foreach (var axiom in axiomSchemes)
                    {
                        if (axiom.IsMatch(expr))
                        {
                            isProofed = true;
                            if (alpha != null)
                            {
                                newProof.AddRange(BaseDeduct(expr, alpha));
                            }
                            break;
                        }
                    }
#endregion 

                    #region isAlpha
                    if (!isProofed)
                    {
                        if (alpha != null && expr.Equals(alpha))
                        {
                            isProofed = true;
                            foreach (var e in selfCons)
                            {
                                newProof.Add(SubstituteExprsToExpr(e, new Dictionary<string, IExpression>() { { "A", expr } }));
                            }
                        }
                    }
                    #endregion
                    #region induction
                    if (!isProofed)
                    {
                        if (inductionAxiom.IsMatch(expr))
                        {
                            try
                            {
                                var f = ((Implication)expr).Right;
                                var conj = (Conjuction)((Implication)expr).Left;
                                var quant = (Quantifier)conj.Right;
                                var x = quant.Variable;
                                if (SubstituteVariableToExpr(f, x, new Zero()).Equals(((ArityOperation)(conj).Arguments[0]))
                                    &&
                                    SubstituteVariableToExpr(f, x, new Increment(x)).Equals(((ArityOperation)quant.Expression).Arguments[1]))
                                {
                                    isProofed = true;
                                    if (alpha != null)
                                    {
                                        newProof.AddRange(BaseDeduct(expr, alpha));
                                    }
                                } 
                            } catch
                            {
                                isProoveCorrect = false;
                                break;
                            }
                        }
                    }
                    #endregion
                    #region universal
                    if (!isProofed)
                    {
                        if (universalAxiom.IsMatch(expr))
                        {
                            var q = (Quantifier)((ArityOperation)(expr)).Arguments[0];

                            var x = q.Variable;
                            var f = q.Expression;
                            //@y(((x)'+y=(x+y)')->((x)'+(y)'=(x+(y)')'))->(x'+0=(x+0)')&@y(((x)'+y=(x+y)')->((x)'+(y)'=(x+(y)')'))
                            IExpression term = null;
                            try
                            {
                                term = GetMismatch(f, ((ArityOperation)expr).Arguments[1], x)?.Clone();
                                try
                                {
                                    if (SubstituteVariableToExpr(f, x, term).Equals(((ArityOperation)expr).Arguments[1]))
                                    {
                                        isProofed = true;
                                        if (alpha != null)
                                        {
                                            newProof.AddRange(BaseDeduct(expr, alpha));
                                        }
                                    }

                                }
                                catch
                                {
                                    reason = $"Терм {term} не свободен для подстановки в формулу {f} вместо {x}";
                                    isProoveCorrect = false;
                                    break;
                                }
                            } catch (SkipException)
                            {

                            }
                            
                        }
                    }
                    #endregion
                    #region existence
                    if (!isProofed)
                    {
                        if (existenceAxiom.IsMatch(expr))
                        {
                            var q = (Quantifier)((ArityOperation)(expr)).Arguments[1];

                            var x = q.Variable;
                            var f = q.Expression;

                            IExpression term = null;
                            try
                            {
                                term = GetMismatch(f, ((ArityOperation)expr).Arguments[0], x)?.Clone();
                                try
                                {
                                    if (SubstituteVariableToExpr(f, x, term).Equals(((ArityOperation)expr).Arguments[0]))
                                    {
                                        isProofed = true;
                                        if (alpha != null)
                                        {
                                            newProof.AddRange(BaseDeduct(expr, alpha));
                                        }
                                    }

                                }
                                catch
                                {
                                    reason = $"Терм {term} не свободен для подстановки в формулу {f} вместо {x}";
                                    isProoveCorrect = false;
                                    break;
                                }
                            } catch (SkipException skip)
                            {

                            }
                        }
                    }
                    #endregion
                    #region M.P
                    if (!isProofed)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (proof[j] is Implication impl && impl.Right.Equals(expr) && cache.ContainsKey(impl.Left))
                            {
                                isProofed = true;
                                if (alpha != null)
                                {
                                    var t = SubstituteExprsToExpr(axiomSchemes[1].Expression, 
                                        new Dictionary<string, IExpression>() {
                                            { "A", alpha },
                                            { "B", impl.Arguments[0] },
                                            { "C", expr }
                                        });
                                    newProof.AddRange(new IExpression[] {
                                        t,
                                        ((ArityOperation)t).Arguments[1],
                                        ((ArityOperation)((ArityOperation)t).Arguments[1]).Arguments[1] });
                                }
                                break;
                            }
                        }
                    }
                    #endregion
                    #region universalRule
                    if (!isProofed)
                    {
                        if (expr is Implication impl && impl.Right is Universal uv)
                        {
                            var ne = new Implication(impl.Left, uv.Expression);
                            var x = uv.Variable;

                            if (cache.ContainsKey(ne))
                            {
                                var isFree = !SubstituteVariableToExpr(impl.Left, x, new Zero()).Equals(impl.Left);
                                if (!isFree)
                                {
                                    isProofed = true;
                                    if (alpha != null)
                                    {
                                        if (SubstituteVariableToExpr(alpha, x, new Zero()).Equals(alpha))
                                        {
                                            using (var sr = new StreamReader(new FileStream("Universal.proof", FileMode.Open)))
                                            {
                                                string s;
                                                while ((s = sr.ReadLine()) != null)
                                                {
                                                    newProof.Add(SubstituteExprsToExpr(parser.Parse(s), new Dictionary<string, IExpression>()
                                                    {

                                                        {"A", alpha },
                                                        {"B", impl.Left },
                                                        {"C", uv.Expression },
                                                        {"x", x }
                                                    }));
                                                }
                                            }
                                        } else
                                        {
                                            isProoveCorrect = false;
                                            reason = $"Переменная {x} входит свободно в формулу {alpha}";
                                            break;
                                        }
                                    }
                                } else
                                {
                                    isProoveCorrect = false;
                                    reason = $"Переменная {x} входит свободно в формулу {impl.Left}";
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                    #region ExistenceRule
                    if (!isProofed)
                    {
                        if (expr is Implication impl && impl.Left is Existence ex)
                        {
                            var ne = new Implication(ex.Expression, impl.Right);
                            var x = ex.Variable;

                            if (cache.ContainsKey(ne))
                            {
                                var isFree = !SubstituteVariableToExpr(impl.Right, x, new Zero()).Equals(impl.Right);
                                if (!isFree)
                                {
                                    isProofed = true;
                                    if (alpha != null)
                                    {
                                        if (SubstituteVariableToExpr(alpha, x, new Zero()).Equals(alpha))
                                        {
                                            using (var sr = new StreamReader(new FileStream("Existence.proof", FileMode.Open)))
                                            {
                                                string s;
                                                while ((s = sr.ReadLine()) != null)
                                                {
                                                    newProof.Add(SubstituteExprsToExpr(parser.Parse(s), new Dictionary<string, IExpression>()
                                                    {

                                                        {"A", alpha },
                                                        {"B",  ex.Expression},
                                                        {"C", impl.Right},
                                                        {"x", x }
                                                    }));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            isProoveCorrect = false;
                                            reason = $"Переменная {x} входит свободно в формулу {alpha}";
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    isProoveCorrect = false;
                                    reason = $"Переменная {x} входит свободно в формулу {impl.Left}";
                                    break;
                                }
                            }
                        }
                    }
#endregion

                    if (!isProofed || !isProoveCorrect)
                    {
                        if (isProoveCorrect)
                        {
                            reason = "Не доказано";
                        }
                        isProoveCorrect = false;
                        break;
                    }
                    cache[expr] = 1;
                }

                using (var sw = new StreamWriter(new FileStream("test.out", FileMode.Create)))
                {
                    if (isProoveCorrect)
                    {
                        if (alpha != null)
                        {
                            proof = newProof;
                            for (int j = 0; j < assumptions.Count - 1; j++)
                            {
                                sw.Write(assumptions[j] + (j != assumptions.Count - 2 ? "," : ""));
                            }
                            sw.Write("|-");
                            sw.WriteLine(new Implication(alpha, toProof));
                        } else
                        {
                            sw.WriteLine(header);
                        }
                        foreach (var p in proof)
                        {
                            sw.WriteLine(p);
                        }
                    }
                    else
                    {
                        sw.WriteLine($"Вывод неверен начиная с формулы номер {i + 1}: [{reason}]");
                    }
                }
            }
        }
    }
}
