using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser.Operators
{
    public class Constant : Expression
    {
        public string Value { get; protected internal set; }

        public Constant(string value)
        {
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Constant a)
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

        public override Expression Clone()
        {
            return new Constant((string)Value.Clone());
        }

    }
}
