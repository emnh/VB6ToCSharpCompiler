using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using io.proleap.vb6.asg.metamodel.impl;

namespace VB6ToCSharpCompiler
{
    public class CompileResult
    {
        public string FileName { get; set; }
        public List<string> ModuleNames { get; } 
        public string VBCode { get; set; }
        public string CSharpCode { get; set; }
        public io.proleap.vb6.asg.metamodel.Program Program { get; set; }

        public CompileResult()
        {
            ModuleNames = new List<string>();
        }
    }
}
