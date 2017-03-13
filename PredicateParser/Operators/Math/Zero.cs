using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Math
{
    public sealed class Zero : IExpression, IMath
    {
        public string Value { get { return "0"; } }

        public Zero()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Zero;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        public IExpression Clone()
        {
            return new Zero();
        }
    }
}
