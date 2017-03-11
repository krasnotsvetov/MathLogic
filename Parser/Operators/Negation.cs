using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser.Operators
{
    public class Negation : UnaryOperation 
    {
        public Negation(Expression expression) : base(expression, "!")
        {

        }

        public override Expression Clone()
        {
            return new Negation(Expression.Clone());
        }
    }
}
