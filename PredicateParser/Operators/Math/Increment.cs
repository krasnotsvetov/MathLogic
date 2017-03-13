using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public sealed class Increment : UnaryOperation, IMath
    {
        public Increment(IExpression expression) : base(expression, "'")
        {

        }

        public override IExpression Clone()
        {
            return new Increment(Expression.Clone());
        }

        public override string ToString()
        {
            
            return Expression.ToString() +"'";
        }
    }
}
