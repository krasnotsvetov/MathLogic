using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            var expr = parser.Parse("A->B->?x(x=x)");
        }
    }
}
