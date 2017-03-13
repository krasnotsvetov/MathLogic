using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public class Equation : BinaryOperation, ILogic
    {
        public Equation(IExpression left, IExpression right) : base(left, right, "=")
        {
        }

        public override IExpression Clone()
        {
            return new Equation(Left.Clone(), Right.Clone());
        }
    }
}
