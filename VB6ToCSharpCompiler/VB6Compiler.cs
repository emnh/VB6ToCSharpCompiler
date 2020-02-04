using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.sun.org.apache.xpath.@internal.functions;
using io.proleap.vb6;
using io.proleap.vb6.asg.metamodel.impl;
using io.proleap.vb6.asg.@params.impl;
using javax.swing.text;
using org.antlr.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public static class VB6Compiler
    {
        public static string[] GetFiles() {
            return Directory. GetFiles("SLPC2");
        }

        public static void Visit(CompileResult compileResult, VisitorCallback callback)
        {
            if (compileResult == null)
            {
                throw new ArgumentNullException(nameof(compileResult));
            }

            var program = compileResult.Program;

            if (compileResult.Program == null)
            {
                throw new ArgumentNullException(nameof(compileResult));
            }

            var modules = program.getModules();

            var visitor = new VB6ASTTreeViewGeneratorVisitor(callback);

            for (int i = 0; i < modules.size(); i++)
            {
                var module = (ModuleImpl)modules.get(i);
                var ctx = module.getCtx();
                visitor.visit(ctx);
            }
        }

        public static CompileResult Compile(string fileName, string data = null, bool translate = true)
        {
            CompileResult returnValue = new CompileResult();

            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (!(fileName.EndsWith(".bas", true, CultureInfo.CurrentCulture) || fileName.EndsWith(".frm", true, CultureInfo.CurrentCulture)))
            {
                {
                    throw new ArgumentException(fileName + " is not a VB6 module");
                }
            }

            var code = data ?? System.IO.File.ReadAllText(fileName);

            // For now, treat forms as modules
            /*
            if (fileName.EndsWith(".frm"))
            {
                var splitted =
                    code.Split(new string[] { "Attribute VB_Name" }, StringSplitOptions.None);
                code = "Attribute VB_Name" + splitted[1];
            }*/

            returnValue.VBCode = code;
            returnValue.FileName = fileName;

            var inputFile = new java.io.File(fileName);

            code = code.Replace("\r", "");

            io.proleap.vb6.asg.metamodel.Program program =
                new io.proleap.vb6.asg.runner.impl.VbParserRunnerImpl().analyzeCode(code, fileName,
                    new VbParserParamsImpl());

            returnValue.Program = program;


            var modules = program.getModules();
            
            for (int i = 0; i < modules.size(); i++)
            {
                var module = (ModuleImpl)modules.get(i);
                var ctx = module.getCtx();
                var modName = module.getName();
                returnValue.ModuleNames.Add(modName);
                // TODO: use Trivia Syntax Elements?
                returnValue.CSharpCode += "// Module Name: " + modName + "\r\n";
                if (translate)
                {
                    var syntaxTree = new Translator(returnValue).Translate(program, module);
                    returnValue.CSharpCode += syntaxTree.ToFullString() + "\r\n";
                }
                //syntaxTree.
                //visitor.visit(((ModuleImpl)modules.get(i)).getCtx());
            }

            return returnValue;
        }
    }

}
