using PredicateParser;
using PredicateParser.Operators;
using PredicateParser.Operators.Logic;
using PredicateParser.Operators.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemD
{
    class AxiomScheme
    {
        public readonly IExpression Expression;
        public AxiomScheme(string expr)
        {
            Expression = new Parser().Parse(expr);
        }


        public AxiomScheme(IExpression expr)
        {
            this.Expression = expr;
        }


        public bool IsMatch(IExpression expr)
        {
            return _isMatch(Expression, expr, new Dictionary<Predicate, IExpression>());
        }

        private bool _isMatch(IExpression axiom, IExpression expr, Dictionary<Predicate, IExpression> map)
        {
            if (expr is IMath)
            {
                return false;
            }
            switch (axiom)
            {
                case Predicate p:
                    if (map.ContainsKey(p))
                    {
                        return map[p].Equals(expr);
                    } else
                    {
                        map[p] = expr;
                        return true;
                    } 
                case ArityOperation ao:
                    if (expr is ArityOperation e && e.Arity == ao.Arity)
                    {
                        for (int i = 0; i < e.Arity; i++)
                        {
                            if (!_isMatch(ao.Arguments[i], e.Arguments[i], map))
                            {
                                return false;
                            }
                        }
                    } else
                    {
                        return false;
                    }
                    return true;
                default:
                    return false;
            }
        }

         
    }
}
