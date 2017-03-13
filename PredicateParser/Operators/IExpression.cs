using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser.Operators
{
    public interface IExpression
    {
        IExpression Clone();
    }
}
