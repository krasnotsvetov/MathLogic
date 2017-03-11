using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser
{
    public class ParserContext
    {
        public string Text;
        public int CurrentPosition;

        public ParserContext(string text, int currentPositon)
        {
            this.Text = text;
            this.CurrentPosition = currentPositon;
        }

    }
}
