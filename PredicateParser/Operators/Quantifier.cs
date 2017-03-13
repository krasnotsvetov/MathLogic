using PredicateParser.Operators.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators
{
    public class Quantifier : IExpression, ILogic
    {
        public Variable Variable { get; set; }
        public IExpression Expression { get; set; }
        public string Function;

        public Quantifier(Variable c, IExpression expr, string function)
        {
            this.Function = function;
            this.Variable = c;
            this.Expression = expr;
        }
        public virtual IExpression Clone()
        {
            return new Quantifier((Variable)Variable.Clone(), Expression.Clone(), (string)Function.Clone());
        }

        public override string ToString()
        {
            return Function + Variable.ToString() + Expression.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Quantifier q)
            {
                return q.Function.Equals(Function) && q.Variable.Equals(Variable) && q.Expression.Equals(Expression);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Function.GetHashCode() * 2039 + Expression.GetHashCode() * 181 + Variable.GetHashCode() * 71;
        }
    }
}
