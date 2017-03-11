using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser
{
    public abstract class BinaryOperation : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public readonly string Function;

        public BinaryOperation(Expression left, Expression right, string function)
        {
            this.Left = left;
            this.Right = right;
            this.Function = function;
        }

        public override string ToString()
        {
            return $"({Left.ToString()}{Function}{Right.ToString()})";
        }

        public override bool Equals(object obj)
        {
            if (obj is BinaryOperation b)
            {
                return this.GetType().Equals(b.GetType()) && Left.Equals(b.Left) && Right.Equals(b.Right);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Function.GetHashCode() * 2039 + Right.GetHashCode() * 181 + Left.GetHashCode() * 71;
        }

    }
}
