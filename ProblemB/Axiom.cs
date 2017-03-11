using LogicParser;
using LogicParser.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemB
{
    class Axiom
    {
        public readonly Expression Expression;
        public Axiom(string expr)
        {
            Expression = new Parser().Parse(expr);
        }

        public Axiom(Expression expr)
        {
            this.Expression = expr;
        }


        public bool IsMatch(Expression expr)
        {
            return _isMatch(Expression, expr, new Dictionary<Constant, Expression>());
        }

        private bool _isMatch(Expression axiom, Expression expr, Dictionary<Constant, Expression> map)
        {
            switch (axiom)
            {
                case Constant c:
                    if (map.ContainsKey(c))
                    {
                        return map[c].Equals(expr);
                    } else
                    {
                        map[c] = expr;
                        return true;
                    } 
                case BinaryOperation b:
                    if (expr is BinaryOperation eb && b.Function.Equals(eb.Function))
                    {
                        return _isMatch(b.Left, eb.Left, map) && _isMatch(b.Right, eb.Right, map);
                    } else
                    {
                        return false;
                    } 
                case UnaryOperation u:
                    if (expr is UnaryOperation eu && u.Function.Equals(u.Function))
                    {
                        return _isMatch(u.Expression, eu.Expression, map);
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

         
    }
}
