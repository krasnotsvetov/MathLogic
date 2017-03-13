using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators
{
    public abstract class UnaryOperation : ArityOperation
    {
        public IExpression Expression { get { return Arguments[0]; } set { Arguments[0] = value; } }
        public UnaryOperation(IExpression expression, string function) : base (function, 1, new List<IExpression>() { expression})
        {
            this.Expression = expression;
        }

        public override string ToString()
        { 
            return $"({Name}{Expression.ToString()})";
        }

        public override bool Equals(object obj)
        {
            if (obj is UnaryOperation u)
            {
                return this.GetType().Equals(u.GetType()) && Expression.Equals(u.Expression);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() * 2039 + Expression.GetHashCode() * 127;
        }

    }
}
