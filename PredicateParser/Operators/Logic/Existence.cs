using PredicateParser.Operators.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public class Existence : Quantifier
    {
        public Existence(Variable v, IExpression expr) : base(v, expr, "?")
        {

        }

        public override IExpression Clone()
        {
            return new Existence((Variable)Variable.Clone(), Expression.Clone());
        }
    }
}
