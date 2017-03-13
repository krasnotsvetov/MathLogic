using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Math
{
    public class Multiplication : BinaryOperation, IMath
    {
        public Multiplication(IExpression left, IExpression right) : base(left, right, "*")
        {
        }

        public override IExpression Clone()
        {
            return new Multiplication(Left.Clone(), Right.Clone());
        }
    }
}
