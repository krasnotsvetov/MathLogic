using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ParserGenerator
{
    public class Generator : IGenerator
    {
        public Generator(string name, string projectDir)
        {
            if (Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
            }
            
        }



      
    }
}
