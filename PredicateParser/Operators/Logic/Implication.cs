using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public sealed class Implication : BinaryOperation, ILogic
    {
        public Implication(IExpression left, IExpression right) : base(left, right, "->")
        {

        }

        public override IExpression Clone()
        {
            return new Implication(Left.Clone(), Right.Clone());
        }
    }
}
