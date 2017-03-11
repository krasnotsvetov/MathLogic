using LogicParser;
using LogicParser.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemC
{
    public static class ProofCollection
    {

        static Dictionary<Type, Dictionary<int, List<string>>> rawProofs = new Dictionary<Type, Dictionary<int, List<string>>>();
        static Dictionary<Type, Dictionary<int, List<Expression>>> Proofs = new Dictionary<Type, Dictionary<int, List<Expression>>>();


        static List<string> rawAOrNotA;
        static List<Expression> AOrNotA;
        static ProofCollection()
        {
            rawProofs.Add(typeof(Negation), new Dictionary<int, List<string>>());
            rawProofs.Add(typeof(Disjunction), new Dictionary<int, List<string>>());
            rawProofs.Add(typeof(Conjuction), new Dictionary<int, List<string>>());
            rawProofs.Add(typeof(Implication), new Dictionary<int, List<string>>());


            rawProofs[typeof(Negation)][0] = new List<string>() { "!A" };
            rawProofs[typeof(Negation)][1] = new List<string>()
            {
                "A",
                "!A->(!A->!A)->!A",
                "!A->!A->!A",
                "(!A->!A->!A)->(!A->(!A->!A)->!A)->(!A->!A)",
                "(!A->(!A->!A)->!A)->(!A->!A)",
                "!A->!A",
                "A->!A->A",
                "!A->A",
                "(!A->A)->(!A->!A)->!!A",
                "(!A->!A)->!!A",
                "!!A"
            };

            rawProofs[typeof(Disjunction)][0] = new List<string>()
            {
                "(A|B->(A))->(A|B->!(A))->!(A|B)",
                "(A->A)->(B->A)->(A|B->A)",
                "A->A->A",
                "(A->A->A)->(A->(A->A)->A)->(A->A)",
                "(A->(A->A)->A)->(A->A)",
                "A->(A->A)->A",
                "A->A",
                "(B->A)->(A|B->A)",
                "(B->!A->B)->B->(B->!A->B)",
                "(B->!A->B)",
                "B->(B->!A->B)",
                "B->B->B",
                "(B->B->B)->(B->(B->B)->B)->(B->B)",
                "(B->(B->B)->B)->(B->B)",
                "B->(B->B)->B",
                "B->B",
                "(B->B)->(B->B->(!A->B))->(B->(!A->B))",
                "(B->B->!A->B)->(B->!A->B)",
                "B->!A->B",
                "(!B->!A->!B)->B->(!B->!A->!B)",
                "!B->!A->!B",
                "B->!B->!A->!B",
                "!B->B->!B",
                "!B",
                "B->!B",
                "(B->!B)->(B->!B->!A->!B)->(B->!A->!B)",
                "(B->!B->!A->!B)->(B->!A->!B)",
                "B->!A->!B",
                "((!A->B)->(!A->!B)->!!A)->B->((!A->B)->(!A->!B)->!!A)",
                "(!A->B)->(!A->!B)->!!A",
                "B->((!A->B)->(!A->!B)->!!A)",
                "(B->(!A->B))->(B->(!A->B)->((!A->!B)->!!A))->(B->((!A->!B)->!!A))",
                "((B->((!A->B)->((!A->!B)->!!A)))->(B->((!A->!B)->!!A)))",
                "B->((!A->!B)->!!A)",
                "(B->(!A->!B))->(B->(!A->!B)->!!A)->(B->!!A)",
                "((B->((!A->!B)->!!A))->(B->!!A))",
                "B->!!A",
                "(!!A->A)->B->(!!A->A)",
                "!!A->A",
                "B->!!A->A",
                "(B->!!A)->(B->!!A->A)->(B->A)",
                "((B->!!A->A)->(B->A))",
                "B->A",
                "A|B->A",
                "(A|B->!A)->!(A|B)",
                "!A->A|B->!A",
                "A|B->!A",
                "!(A|B)"
            };

            rawProofs[typeof(Disjunction)][1] = new List<string>()
            {
                "B->A|B",
                "B",
                "A|B"
            };

            rawProofs[typeof(Disjunction)][2] = new List<string>()
            {
                "A->A|B",
                "A",
                "A|B"
            };

            rawProofs[typeof(Disjunction)][3] = new List<string>()
            {
                "A->A|B",
                "A",
                "A|B"
            };


            rawProofs[typeof(Conjuction)][0] = new List<String>()
            {
                "((A&B)->A)->((A&B)->!A)->!(A&B)",
                "(A&B)->A ",
                "(A&B->!A)->!(A&B)",
                "!A->A&B->!A",
                "!A",
                "A&B->!A",
                "!(A&B)"
            };

            rawProofs[typeof(Conjuction)][1] = new List<String>()
            {
                "((A&B)->A)->((A&B)->!A)->!(A&B)",
                "(A&B)->A ",
                "(A&B->!A)->!(A&B)",
                "!A->A&B->!A",
                "!A",
                "A&B->!A",
                "!(A&B)"
            };


            rawProofs[typeof(Conjuction)][2] = new List<String>()
            {
                "((A&B)->B)->((A&B)->!B)->!(A&B)",
                "(A&B)->B ",
                "(A&B->!B)->!(A&B)",
                "!B->A&B->!B",
                "!B",
                "A&B->!B",
                "!(A&B)"
            };

            rawProofs[typeof(Conjuction)][3] = new List<String>()
            {
                "A->B->A&B",
                "A",
                "B->A&B",
                "B",
                "A&B"
            };

            rawProofs[typeof(Implication)][0] = new List<String>()
            {
                "(A->!B->A)->A->(A->!B->A)",
                "(A->!B->A)",
                "A->(A->!B->A)",
                "A->A->A",
                "(A->A->A)->(A->(A->A)->A)->(A->A)",
                "(A->(A->A)->A)->(A->A)",
                "A->(A->A)->A",
                "A->A",
                "(A->A)->(A->A->(!B->A))->(A->(!B->A))",
                "(A->A->!B->A)->(A->!B->A)",
                "A->!B->A",
                "(!A->!B->!A)->A->(!A->!B->!A)",
                "!A->!B->!A",
                "A->!A->!B->!A",
                "!A->A->!A",
                "!A",
                "A->!A",
                "(A->!A)->(A->!A->!B->!A)->(A->!B->!A)",
                "(A->!A->!B->!A)->(A->!B->!A)",
                "A->!B->!A",
                "((!B->A)->(!B->!A)->!!B)->A->((!B->A)->(!B->!A)->!!B)",
                "(!B->A)->(!B->!A)->!!B",
                "A->((!B->A)->(!B->!A)->!!B)",
                "(A->(!B->A))->(A->(!B->A)->((!B->!A)->!!B))->(A->((!B->!A)->!!B))",
                "((A->((!B->A)->((!B->!A)->!!B)))->(A->((!B->!A)->!!B)))",
                "A->((!B->!A)->!!B)",
                "(A->(!B->!A))->(A->(!B->!A)->!!B)->(A->!!B)",
                "((A->((!B->!A)->!!B))->(A->!!B))",
                "A->!!B",
                "(!!B->B)->A->(!!B->B)",
                "!!B->B",
                "A->!!B->B",
                "(A->!!B)->(A->!!B->B)->(A->B)",
                "((A->!!B->B)->(A->B))",
                "A->B"
            };

            rawProofs[typeof(Implication)][1] = new List<String>()
            {
                "B->A->B",
                "B",
                "A->B"
            };

            rawProofs[typeof(Implication)][2] = new List<String>()
            {
                "((A->B)->B)->((A->B)->!B)->!(A->B)",
                "((A->B)->A)->((A->B)->(A->B))->((A->B)->B)",
                "A->(A->B)->A",
                "A",
                "(A->B)->A",
                "((A->B)->(A->B))->((A->B)->B)",
                "((A->B)->(A->B)->(A->B))",
                "((A->B)->(A->B)->(A->B))->((A->B)->((A->B)->(A->B))->(A->B))->((A->B)->(A->B))",
                "((A->B)->((A->B)->(A->B))->(A->B))->((A->B)->(A->B))",
                "((A->B)->((A->B)->(A->B))->(A->B))",
                "(A->B)->(A->B)",
                "(A->B)->B",
                "!B->(A->B)->!B",
                "!B",
                "(A->B)->!B",
                "((A->B)->!B)->!(A->B)",
                "!(A->B)"
            };

            rawProofs[typeof(Implication)][3] = new List<String>()
            {
                "B->A->B",
                "B",
                "A->B"
            };

            rawAOrNotA = new List<string>()
            {
                "A->A|!A",
                "(((A->(A|!A))->((A->!(A|!A))->!A))->(!(A|!A)->((A->(A|!A))->((A->!(A|!A))->!A))))",
                "((A->(A|!A))->((A->!(A|!A))->!A))",
                "(!(A|!A)->((A->(A|!A))->((A->!(A|!A))->!A)))",
                "((A->(A|!A))->(!(A|!A)->(A->(A|!A))))",
                "(A->(A|!A))",
                "(!(A|!A)->(A->(A|!A)))",
                "((!(A|!A)->(A->(A|!A)))->((!(A|!A)->((A->(A|!A))->((A->!(A|!A))->!A)))->(!(A|!A)->((A->!(A|!A))->!A))))",
                "((!(A|!A)->((A->(A|!A))->((A->!(A|!A))->!A)))->(!(A|!A)->((A->!(A|!A))->!A)))",
                "(!(A|!A)->((A->!(A|!A))->!A))",
                "((!(A|!A)->(A->!(A|!A)))->(!(A|!A)->(!(A|!A)->(A->!(A|!A)))))",
                "(!(A|!A)->(A->!(A|!A)))",
                "(!(A|!A)->(!(A|!A)->(A->!(A|!A))))",
                "(!(A|!A)->(!(A|!A)->!(A|!A)))",
                "((!(A|!A)->(!(A|!A)->!(A|!A)))->((!(A|!A)->((!(A|!A)->!(A|!A))->!(A|!A)))->(!(A|!A)->!(A|!A))))",
                "((!(A|!A)->((!(A|!A)->!(A|!A))->!(A|!A)))->(!(A|!A)->!(A|!A)))",
                "(!(A|!A)->((!(A|!A)->!(A|!A))->!(A|!A)))",
                "(!(A|!A)->!(A|!A))",
                "((!(A|!A)->!(A|!A))->((!(A|!A)->(!(A|!A)->(A->!(A|!A))))->(!(A|!A)->(A->!(A|!A)))))",
                "((!(A|!A)->(!(A|!A)->(A->!(A|!A))))->(!(A|!A)->(A->!(A|!A))))",
                "(!(A|!A)->(A->!(A|!A)))",
                "((!(A|!A)->(A->!(A|!A)))->((!(A|!A)->((A->!(A|!A))->!A))->(!(A|!A)->!A)))",
                "((!(A|!A)->((A->!(A|!A))->!A))->(!(A|!A)->!A))",
                "(!(A|!A)->!A)",
                "!A->A|!A",
                "(((!A->(A|!A))->((!A->!(A|!A))->!!A))->(!(A|!A)->((!A->(A|!A))->((!A->!(A|!A))->!!A))))",
                "((!A->(A|!A))->((!A->!(A|!A))->!!A))",
                "(!(A|!A)->((!A->(A|!A))->((!A->!(A|!A))->!!A)))",
                "((!A->(A|!A))->(!(A|!A)->(!A->(A|!A))))",
                "(!A->(A|!A))",
                "(!(A|!A)->(!A->(A|!A)))",
                "((!(A|!A)->(!A->(A|!A)))->((!(A|!A)->((!A->(A|!A))->((!A->!(A|!A))->!!A)))->(!(A|!A)->((!A->!(A|!A))->!!A))))",
                "((!(A|!A)->((!A->(A|!A))->((!A->!(A|!A))->!!A)))->(!(A|!A)->((!A->!(A|!A))->!!A)))",
                "(!(A|!A)->((!A->!(A|!A))->!!A))",
                "((!(A|!A)->(!A->!(A|!A)))->(!(A|!A)->(!(A|!A)->(!A->!(A|!A)))))",
                "(!(A|!A)->(!A->!(A|!A)))",
                "(!(A|!A)->(!(A|!A)->(!A->!(A|!A))))",
                "(!(A|!A)->(!(A|!A)->!(A|!A)))",
                "((!(A|!A)->(!(A|!A)->!(A|!A)))->((!(A|!A)->((!(A|!A)->!(A|!A))->!(A|!A)))->(!(A|!A)->!(A|!A))))",
                "((!(A|!A)->((!(A|!A)->!(A|!A))->!(A|!A)))->(!(A|!A)->!(A|!A)))",
                "(!(A|!A)->((!(A|!A)->!(A|!A))->!(A|!A)))",
                "(!(A|!A)->!(A|!A))",
                "((!(A|!A)->!(A|!A))->((!(A|!A)->(!(A|!A)->(!A->!(A|!A))))->(!(A|!A)->(!A->!(A|!A)))))",
                "((!(A|!A)->(!(A|!A)->(!A->!(A|!A))))->(!(A|!A)->(!A->!(A|!A))))",
                "(!(A|!A)->(!A->!(A|!A)))",
                "((!(A|!A)->(!A->!(A|!A)))->((!(A|!A)->((!A->!(A|!A))->!!A))->(!(A|!A)->!!A)))",
                "((!(A|!A)->((!A->!(A|!A))->!!A))->(!(A|!A)->!!A))",
                "(!(A|!A)->!!A)",
                "(!(A|!A)->!A)->(!(A|!A)->!!A)->!!(A|!A)",
                "(!(A|!A)->!!A)->!!(A|!A)",
                "!!(A|!A)",
                "!!(A|!A)->(A|!A)",
                "A|!A"
            };

            MakeProofs();
        }

        private static void MakeProofs()
        {
            var parser = new Parser();
            foreach (var key in rawProofs.Keys)
            {
                Proofs.Add(key, new Dictionary<int, List<Expression>>());
                foreach (var key2 in rawProofs[key].Keys)
                {
                    Proofs[key][key2] = new List<Expression>();
                    foreach (var e in rawProofs[key][key2])
                    {
                        Proofs[key][key2].Add(parser.Parse(e));
                    }
                }
            }

            AOrNotA = new List<Expression>();
            foreach (var s in rawAOrNotA)
            {
                AOrNotA.Add(parser.Parse(s));
            }
        }


        public static List<Expression> GetAOrNotA()
        {
            var rv = new List<Expression>();
            foreach (var e in AOrNotA)
            {
                rv.Add(e.Clone());
            }
            return rv;
        }

        public static List<Expression> GetProof(Type type, params bool[] b)
        {
            var rv = new List<Expression>();
            int index = 0;
            if (b.Length == 1)
            {
                if (b[0])
                {
                    index = 1;
                } else
                {
                    index = 0;
                }
            } else
            {
                if (b[0] && b[1])
                {
                    index = 3;
                }

                if (b[0] && !b[1])
                {
                    index = 2;
                }

                if (!b[0] && b[1])
                {
                    index = 1;
                }

                if (!b[0] && !b[1])
                {
                    index = 0;
                }
            }

            var source = Proofs[type][index];
            foreach (var s in source)
            {
                rv.Add(s.Clone());
            }
            return rv;
        }
    }
}
