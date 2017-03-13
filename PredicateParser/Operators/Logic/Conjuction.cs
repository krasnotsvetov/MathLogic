using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public sealed class Conjuction : BinaryOperation, ILogic
    {
        public Conjuction(IExpression left, IExpression right) : base(left, right, "&")
        {

        }

        public override IExpression Clone()
        {
            return new Conjuction(Left.Clone(), Right.Clone());
        }
    }
}
