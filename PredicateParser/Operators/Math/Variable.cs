using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Math
{
    public sealed class Variable : IExpression, IMath
    {
        public string Value { get; internal set; }

        public Variable(string value)
        {
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Variable a)
            {
                return a.Value.Equals(Value);
            }
            if (obj is string)
            {
                return Value.Equals(obj);
            }
            return base.Equals(obj);
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
            return new Variable((string)Value.Clone());
        }

    }
}
