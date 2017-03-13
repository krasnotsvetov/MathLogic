using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators.Logic
{
    public class Predicate : ArityOperation, ILogic
    {
        public Predicate(string name, int arity, List<IExpression> args) : base(name, arity, args)
        {

        }

        public override string ToString()
        {
            if (Arity == 0)
            {
                return Name;
            }
            return base.ToString();
        }

        public override IExpression Clone()
        {
            List<IExpression> copyArg = new List<IExpression>(Arity);
            foreach (var a in Arguments)
            {
                copyArg.Add(a.Clone());
            }
            return new Predicate((string)Name.Clone(), Arity, copyArg);
        } 
    }
}
