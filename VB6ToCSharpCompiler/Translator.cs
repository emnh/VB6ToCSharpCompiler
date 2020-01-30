using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using io.proleap.vb6.asg.metamodel.impl;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VB6ToCSharpCompiler
{
    public static class Translator
    {
        public static CompilationUnitSyntax Translate(ModuleImpl module)
        {

            return SyntaxFactory.CompilationUnit();
        }
    }
}
