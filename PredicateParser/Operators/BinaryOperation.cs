using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators
{
    public abstract class BinaryOperation : ArityOperation
    {
        public IExpression Left { get {return Arguments[0];} set { Arguments[0] = value;} }
        public IExpression Right { get { return Arguments[1]; } set { Arguments[1] = value; } }

        public BinaryOperation(IExpression left, IExpression right, string function) : base(function, 2, new List<IExpression>() { left, right })
        {
            this.Left = left;
            this.Right = right;
        }

        public override string ToString()
        {
            return $"({Left.ToString()}{Name}{Right.ToString()})";
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
            return Name.GetHashCode() * 2039 + Right.GetHashCode() * 181 + Left.GetHashCode() * 71;
        }

    }
}
