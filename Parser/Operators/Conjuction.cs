using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser.Operators
{
    public class Conjuction : BinaryOperation
    {
        public Conjuction(Expression left, Expression right) : base(left, right, "&")
        {

        }

        public override Expression Clone()
        {
            return new Conjuction(Left.Clone(), Right.Clone());
        }
    }
}
