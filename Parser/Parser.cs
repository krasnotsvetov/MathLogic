using LogicParser.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicParser
{
    public class Parser
    {
        //TODO : ParserSettings
        public Parser()
        {

        }

        private bool skipIf(Predicate<char> skipper, ParserContext context)
        {
            bool rv = false;
            if (context.CurrentPosition < context.Text.Length && skipper(context.Text[context.CurrentPosition]))
            {
                rv = true;
                context.CurrentPosition++;
            }
            return rv;
        }

        private bool skipWhile(Predicate<char> skipper, ParserContext context)
        {
            bool rv = false;
            while (context.CurrentPosition < context.Text.Length && skipper(context.Text[context.CurrentPosition]))
            {
                rv = true;
                context.CurrentPosition++;
            }
            return rv;
        }

        private bool skipWhiteSpace(ParserContext context)
        {
            return skipWhile((c) => char.IsWhiteSpace(c), context);
        }

        private bool skipCharacter(char toSkip, ParserContext context)
        {
            return skipIf((c) => c == toSkip, context);
        }

        private bool skipASymbol(ParserContext context)
        {
            return skipIf((c) => true, context);
        }

        public Expression Parse(string value)
        { 
            ParserContext ctx = new ParserContext(value, 0);
            return GetExpression(ctx);
        }


        private Expression GetExpression(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            var a = GetDisjunction(ctx);
            if (ctx.CurrentPosition != ctx.Text.Length && skipCharacter('-', ctx))
            {
                skipASymbol(ctx);
                a = new Implication(a, GetExpression(ctx));
            }
            skipWhiteSpace(ctx);
            return a;
        }

        private Expression GetDisjunction(ParserContext ctx)
        { 
            skipWhiteSpace(ctx);
            var a = GetConjuction(ctx);
            while (ctx.CurrentPosition != ctx.Text.Length && skipCharacter('|', ctx))
            { 
                a = new Disjunction(a, GetConjuction(ctx));
            }
            skipWhiteSpace(ctx);
            return a;
        }

        private Expression GetConjuction(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            var a = GetNegation(ctx);
            while (ctx.CurrentPosition != ctx.Text.Length && skipCharacter('&', ctx))
            { 
                a = new Conjuction(a, GetNegation(ctx));
            }
            skipWhiteSpace(ctx);
            return a;
        }

        private Expression GetNegation(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            if (ctx.CurrentPosition == ctx.Text.Length)
            {
                //throw exception
            }
            
            if (skipCharacter('!', ctx))
            {
                skipWhiteSpace(ctx);
                return new Negation(GetNegation(ctx));
            }

            if (skipCharacter('(', ctx))
            {
                var rv = GetExpression(ctx);
                if (!skipCharacter(')', ctx))
                {
                    //TODO: exception
                }
                skipWhiteSpace(ctx);
                return rv;
            }

            var value = "";
            while (ctx.CurrentPosition != ctx.Text.Length && char.IsLetterOrDigit(ctx.Text[ctx.CurrentPosition]))
            {
                value += ctx.Text[ctx.CurrentPosition];
                skipASymbol(ctx);
                
            }
            skipWhiteSpace(ctx);
            return new Constant(value);
        }
    }
}
