using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Math
{
    public class Addition : BinaryOperation, IMath
    {
        public Addition(IExpression left, IExpression right) : base(left, right, "+")
        {
        }

        public override IExpression Clone()
        {
            return new Addition(Left.Clone(), Right.Clone());
        }
    }
}
