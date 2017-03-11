using LogicParser;
using LogicParser.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemC
{
    public class Evaluator
    {
        public static List<Constant> GetAllVariables(Expression expression)
        {
            var a = new List<Constant>();

            switch (expression)
            {
                case Constant c:
                    a.Add(c);
                    break;
                case BinaryOperation bo:
                    a.AddRange(GetAllVariables(bo.Left));
                    a.AddRange(GetAllVariables(bo.Right));
                    break;
                case UnaryOperation uo:
                    a.AddRange(GetAllVariables(uo.Expression));
                    break;
            }

            a.Sort((x, y) => x.Value.CompareTo(y.Value));
            return a.Distinct().ToList();
        }

        public static bool Evaluate(Expression expr, Dictionary<Constant, bool> constants)
        {
            switch (expr)
            {
                case Constant c:
                    return constants[c];
                case Negation n:
                    return !Evaluate(n.Expression, constants);
                case Disjunction d:
                    return Evaluate(d.Left, constants) || Evaluate(d.Right, constants);
                case Conjuction c:
                    return Evaluate(c.Left, constants) & Evaluate(c.Right, constants);
                case Implication i:
                    return !Evaluate(i.Left, constants) || Evaluate(i.Right, constants);
                default:
                    throw new Exception();
            }
        }
    }
}
