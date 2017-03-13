using PredicateParser;
using PredicateParser.Operators;
using PredicateParser.Operators.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemD
{
    class Axiom
    {
        public readonly IExpression Expression;
        public Axiom(string expr)
        {
            Expression = new Parser().Parse(expr);
        }

        public Axiom(IExpression expr)
        {
            this.Expression = expr;
        }

    }
}
