using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class CompileResult
    {
        public string FileName { get; set; }
        public List<string> ModuleNames { get; } 
        public string VBCode { get; set; }
        public string CSharpCode { get; set; }

        public CompileResult()
        {
            ModuleNames = new List<string>();
        }
    }
}
