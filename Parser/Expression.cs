using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser
{
    public abstract class Expression
    {
        public string Tag { get; set; }

        public abstract Expression Clone();
    }
}
