using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace VB6ToCSharpCompiler
{
    public class CustomCSharpSyntaxWalker : CSharpSyntaxWalker
    {
        public CsharpVisitorCallback VisitorCallback { get; }

        public CustomCSharpSyntaxWalker(CsharpVisitorCallback callback)
        {
            VisitorCallback = callback;
        }

        public override void Visit(SyntaxNode node)
        {
            VisitorCallback.Callback(node);
            base.Visit(node);
        }
    }
}
