using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser
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

        public ParserContext Clone()
        {
            return new ParserContext((string)Text.Clone(), CurrentPosition);
        }

    }
}
