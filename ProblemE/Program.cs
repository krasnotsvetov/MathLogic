using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemE
{
    class Program
    {
        static void Main(string[] args)
        {
            int a, b;

            using (var sr = new StreamReader(new FileStream("test.in", FileMode.Open)))
            {
                var s = sr.ReadLine().Split(' ').Select(t => int.Parse(t)).ToList();
                a = s[0];
                b = s[1];
            }


            List<string> proofs = new List<string>();
            using (var sr = new StreamReader(new FileStream("base.proof", FileMode.Open)))
            {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    proofs.Add(line);
                }
            }

            string numeralFromInt(int num)
            {
                var sb = new StringBuilder();
                sb.Append("0");
                for (int i = 0; i < num; i++)
                {
                    sb.Append("'");
                }
                return sb.ToString();
            }
            string numeralA = numeralFromInt(a);
            using (var sw = new StreamWriter(new FileStream("test.out", FileMode.Create)))
            {
                sw.WriteLine($"|-{numeralFromInt(a)}+{numeralFromInt(b)}={numeralFromInt(a + b)}");
                using (var sr = new StreamReader(new FileStream("base2.proof", FileMode.Open)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(line);
                    }
                }

                for (int i = 1; i <= a; i++)
                using (var sr = new StreamReader(new FileStream("base3.proof", FileMode.Open)))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(line.Replace("z", numeralFromInt(i)));
                    }
                }
                for (int i = 0; i < b; i++)
                {
                    string numeralB = numeralFromInt(i);
                    string numeralC = numeralFromInt(i + a);
                    foreach (var l in proofs)
                    {
                        foreach (var c in l)
                        {
                            switch (c)
                            {
                                case 'd':
                                    sw.Write(numeralA);
                                    break;
                                case 'e':
                                    sw.Write(numeralB);
                                    break;
                                case 'f':
                                    sw.Write(numeralC);
                                    break;
                                default:
                                    sw.Write(c);
                                    break;
                            }
                        }
                        sw.WriteLine();
                    }
                }
            }

              
        }
    }
}
