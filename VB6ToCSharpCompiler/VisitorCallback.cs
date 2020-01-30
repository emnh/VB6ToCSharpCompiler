using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.sun.org.glassfish.external.amx;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class VisitorCallback
    {
        public Action<ParseTree, ParseTree> Callback { get; set; }
    }
}
