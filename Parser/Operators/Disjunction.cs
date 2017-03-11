using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser.Operators
{
    public class Disjunction : BinaryOperation
    {
        public Disjunction(Expression left, Expression right) : base(left, right, "|")
        {

        }

        public override Expression Clone()
        {
            return new Disjunction(Left.Clone(), Right.Clone());
        }
    }
}
