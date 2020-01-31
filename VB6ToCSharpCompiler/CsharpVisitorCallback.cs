using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace VB6ToCSharpCompiler
{
    public class CsharpVisitorCallback
    {
        public Action<SyntaxNode> Callback { get; set; }
    }
}
