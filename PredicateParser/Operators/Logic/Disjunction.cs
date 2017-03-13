using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public sealed class Disjunction : BinaryOperation, ILogic
    {
        public Disjunction(IExpression left, IExpression right) : base(left, right, "|")
        {

        }

        public override IExpression Clone()
        {
            return new Disjunction(Left.Clone(), Right.Clone());
        }
    }
}
