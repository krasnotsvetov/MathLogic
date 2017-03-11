using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser.Operators
{
    public class Implication : BinaryOperation
    {
        public Implication(Expression left, Expression right) : base(left, right, "->")
        {

        }

        public override Expression Clone()
        {
            return new Implication(Left.Clone(), Right.Clone());
        }
    }
}
