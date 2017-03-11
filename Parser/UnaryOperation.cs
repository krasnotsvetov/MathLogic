using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser
{
    public abstract class UnaryOperation : Expression
    {
        public Expression Expression { get; set; }
        public readonly string Function;
        public UnaryOperation(Expression expression, string function)
        {
            this.Expression = expression;
            this.Function = function;
        }

        public override string ToString()
        { 
            return $"({Function}{Expression.ToString()})";
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
            return Function.GetHashCode() * 2039 + Expression.GetHashCode() * 127;
        }
    }
}
