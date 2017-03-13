using PredicateParser.Operators;
using PredicateParser.Operators.Logic;
using PredicateParser.Operators.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredicateParser
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

        public IExpression Parse(string value)
        { 
            ParserContext ctx = new ParserContext(value, 0);
            return GetExpression(ctx);
        }


        private IExpression GetExpression(ParserContext ctx)
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

        private IExpression GetDisjunction(ParserContext ctx)
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

        private IExpression GetConjuction(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            var a = GetUnary(ctx);
            while (ctx.CurrentPosition != ctx.Text.Length && skipCharacter('&', ctx))
            { 
                a = new Conjuction(a, GetUnary(ctx));
            }
            skipWhiteSpace(ctx);
            return a;
        }

        private IExpression GetUnary(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            if (ctx.CurrentPosition == ctx.Text.Length)
            {
                //throw exception
            }
            
            if (skipCharacter('!', ctx))
            {
                skipWhiteSpace(ctx);
                return new Negation(GetUnary(ctx));
            }

            if (skipCharacter('(', ctx))
            {
                var saveCtx = ctx.Clone();
                try
                {
                    var expr = GetExpression(ctx);
                    if (expr is IMath)
                    {
                        ctx = saveCtx;
                        ctx.CurrentPosition--;
                        return GetPredicate(ctx);
                    }
                    else
                    {
                        skipASymbol(ctx);
                        return expr;
                    }
                }
                catch
                {

                }
                ctx = saveCtx;
                ctx.CurrentPosition--;
                return GetPredicate(ctx);
            }

            var prevChar = ctx.Text[ctx.CurrentPosition];
            if (skipCharacter('@', ctx) || skipCharacter('?', ctx))
            {
                var value = "";
                while (ctx.CurrentPosition != ctx.Text.Length && (char.IsLower(ctx.Text[ctx.CurrentPosition])
                    || char.IsDigit(ctx.Text[ctx.CurrentPosition])))

                {
                    value += ctx.Text[ctx.CurrentPosition];
                    skipASymbol(ctx);

                }
                var constant = new Variable(value);
                if (prevChar == '@')
                {
                    skipWhiteSpace(ctx);
                    return new Universal(constant, GetUnary(ctx));
                } else
                {
                    skipWhiteSpace(ctx);
                    return new Existence(constant, GetUnary(ctx));
                }
            }

            skipWhiteSpace(ctx);
            return GetPredicate(ctx);
        }


        private IExpression GetPredicate(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            var curChar = ctx.Text[ctx.CurrentPosition];
            if (char.IsUpper(curChar))
            {
                var value = "";
                while (ctx.CurrentPosition != ctx.Text.Length && char.IsLetterOrDigit(ctx.Text[ctx.CurrentPosition]))
                {
                    value += ctx.Text[ctx.CurrentPosition];
                    skipASymbol(ctx);

                }
                skipWhiteSpace(ctx);
                List<IExpression> args = new List<IExpression>();
                if (skipCharacter('(', ctx))
                {
                    args.Add(GetTerm(ctx));
                    skipWhiteSpace(ctx);
                    while (skipCharacter(',', ctx))
                    {
                        args.Add(GetTerm(ctx));
                        skipWhiteSpace(ctx);
                    }
                    skipASymbol(ctx);
                }
                skipWhiteSpace(ctx);
                return new Predicate(value,args.Count, args);
            } else
            {
                var first = GetTerm(ctx);
                skipWhiteSpace(ctx);
                if (!skipCharacter('=', ctx))
                {
                    throw new Exception();
                }
                return new Equation(first, GetTerm(ctx));
            }
        }

        private IExpression GetTerm(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            var t = GetAdd(ctx);
            skipWhiteSpace(ctx);
            while (skipCharacter('+', ctx))
            {
                t = new Addition(t, GetAdd(ctx));
                skipWhiteSpace(ctx);
            }
            return t;
        }

        private IExpression GetAdd(ParserContext ctx)
        {
            skipWhiteSpace(ctx);
            var t = GetMul(ctx);
            skipWhiteSpace(ctx);
            while (skipCharacter('*', ctx))
            {
                t = new Multiplication(t, GetMul(ctx));
                skipWhiteSpace(ctx);
            }
            return t;
        }

        private IExpression GetMul(ParserContext ctx)
        {
            IExpression rv;
            skipWhiteSpace(ctx);
            var curChar = ctx.Text[ctx.CurrentPosition];
            if (char.IsLetter(curChar) && char.IsLower(curChar))
            {
                var value = "";
                while (ctx.CurrentPosition != ctx.Text.Length && char.IsLetterOrDigit(ctx.Text[ctx.CurrentPosition]))
                {
                    value += ctx.Text[ctx.CurrentPosition];
                    skipASymbol(ctx);
                }
                skipWhiteSpace(ctx);
                List<IExpression> args = new List<IExpression>();
                if (skipCharacter('(', ctx))
                {
                    args.Add(GetTerm(ctx));
                    skipWhiteSpace(ctx);
                    while (skipCharacter(',', ctx))
                    {
                        args.Add(GetTerm(ctx));
                        skipWhiteSpace(ctx);
                    }
                    skipASymbol(ctx);
                    rv = new Function(value, args.Count, args);
                }
                else
                {
                    rv = new Variable(value);
                }
            } else if (skipCharacter('(', ctx))
            {
                rv = GetTerm(ctx);
                skipWhiteSpace(ctx);
                skipASymbol(ctx);
            } else if (skipCharacter('0', ctx))
            {
                rv = new Zero();
            } else
            {
                throw new Exception();
            }
            skipWhiteSpace(ctx);


            while (skipCharacter('\'', ctx))
            {
                rv = new Increment(rv);
            }
            skipWhiteSpace(ctx);
            return rv;
        }
    }
}
