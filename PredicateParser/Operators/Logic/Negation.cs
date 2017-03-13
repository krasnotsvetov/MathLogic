using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public sealed class Negation : UnaryOperation, ILogic
    {
        public Negation(IExpression expression) : base(expression, "!")
        {

        }

        public override IExpression Clone()
        {
            return new Negation(Expression.Clone());
        }
    }
}
