using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Math
{
    public sealed class Function : ArityOperation, IMath
    {
        public Function(string name, int arity, List<IExpression> args) : base(name, arity, args)
        {

        }

        public override IExpression Clone()
        {
            List<IExpression> copyArg = new List<IExpression>(Arity);
            foreach (var a in Arguments)
            {
                copyArg.Add(a.Clone());
            }
            return new Function((string)Name.Clone(), Arity, copyArg);
        } 
    }
}
