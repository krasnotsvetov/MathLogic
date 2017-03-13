using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
namespace PredicateParser.Operators
{
    public abstract class ArityOperation : IExpression
    {

        public string Name;
        public int Arity { get; private set; }
        public List<IExpression> Arguments { get; private set; }
        public ArityOperation(string name, int arity, List<IExpression> arguments)
        {
            this.Name = name;
            this.Arity = arity;
            this.Arguments = arguments;
        }

        public abstract IExpression Clone();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name).Append("(");
            foreach (var a in Arguments)
            {
                sb.Append(a.ToString()).Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }


        public override bool Equals(object obj)
        {
            if (obj is ArityOperation ao)
            {
                if (Name.Equals(ao.Name) && ao.Arity.Equals(Arity))
                {
                    for (int i = 0; i < Arity; i++)
                    {
                        if (ao.Arguments[i].Equals(Arguments[i]))
                        {
                            return false;
                        }
                    }
                } else
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            for (int i = 0; i < Arity; i++)
            {
                hash += Arguments[i].GetHashCode() * (int)Pow(19, i + 1);   
                hash %= 1_000_000_007;
            }
            return hash;
        }
    }
}
