using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using io.proleap.vb6.asg.metamodel.impl;
using io.proleap.vb6.asg.@params.impl;
using org.antlr.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public static class Compiler
    {
        public static string[] GetFiles() {
            return Directory. GetFiles("SLPC2");
        }

        public static CompileResult Compile(string fileName)
        {
            CompileResult returnValue = new CompileResult();

            if (fileName == null)
            {
                throw new ArgumentException(nameof(fileName) + " is null");
            }

            if (!(fileName.EndsWith(".bas", true, CultureInfo.CurrentCulture) || fileName.EndsWith(".frm", true, CultureInfo.CurrentCulture)))
            {
                {
                    throw new ArgumentException(fileName + " is not a VB6 module");
                }
            }

            var code = System.IO.File.ReadAllText(fileName);

            returnValue.VBCode = code;
            returnValue.FileName = fileName;

            var inputFile = new java.io.File(fileName);

            io.proleap.vb6.asg.metamodel.Program program = new io.proleap.vb6.asg.runner.impl.VbParserRunnerImpl().analyzeFile(inputFile);
            
            var modules = program.getModules();
            
            for (int i = 0; i < modules.size(); i++)
            {
                var module = (ModuleImpl)modules.get(i);
                var ctx = module.getCtx();
                var modName = module.getName();
                returnValue.ModuleNames.Add(modName);
                // TODO: use Trivia Syntax Elements?
                returnValue.CSharpCode += "// Module Name: " + modName + "\r\n";
                var syntaxTree = Translator.Translate(module);
                //syntaxTree.
                //visitor.visit(((ModuleImpl)modules.get(i)).getCtx());
            }

            return returnValue;
        }
    }

}
