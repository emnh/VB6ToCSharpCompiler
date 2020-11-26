using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using com.sun.org.apache.xpath.@internal.functions;
using ikvm.extensions;
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

        public static string MultilineCloseReplacement(Match m)
        {
            if (m is null)
            {
                throw new ArgumentNullException(nameof(m));
            }

            string x = m.ToString();
            if (!x.Contains(","))
            {
                return x;
            }

            x = x.Trim();
            var parts = x.split(@"\s+", 2);
            var parts2 = parts[1].split(",");
            string join = Translator.NewLine;
            foreach (var part in parts2)
            {
                join += parts[0].Trim() + " " + part.Trim() + Translator.NewLine;
            }
            Console.Error.WriteLine("CLOSE: " + m.toString() + ", JOIN: " + join);
            return join;
            //return x.ToUpper();
        }

        public static CompileResult Compile(string fileName, string data = null, bool translate = true)
        {
            CompileResult compileResult = new CompileResult();

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

            var code = data ?? System.IO.File.ReadAllText(fileName, Encoding.GetEncoding(1252));
            //var code = data ?? System.IO.File.ReadAllText(fileName);
            //var code = data ?? System.IO.File.ReadAllText(fileName, Encoding.GetEncoding("ISO-8859-1"));

            // For now, treat forms as modules
            /*
            if (fileName.EndsWith(".frm"))
            {
                var splitted =
                    code.Split(new string[] { "Attribute VB_Name" }, StringSplitOptions.None);
                code = "Attribute VB_Name" + splitted[1];
            }*/

            compileResult.VBCode = code;
            compileResult.FileName = fileName;

            // Workaround a bug in the ProLeap parser with regard to multiple files closed on same line
            code =
                Regex.Replace(
                    code,
                    @"\n\s*Close\s*[^\n]*",
                    MultilineCloseReplacement,
                    RegexOptions.IgnoreCase);

            //Console.Error.WriteLine("CODE: " + code);

            code = code.Replace("\r", "");

            var parserImpl = new VbParserParamsImpl();

            //parserImpl.setCharset(java.nio.charset.Charset.forName("Cp1252"));
            //parserImpl.setCharset(java.nio.charset.Charset.forName("UTF16"));

            io.proleap.vb6.asg.metamodel.Program program =
                new io.proleap.vb6.asg.runner.impl.VbParserRunnerImpl().analyzeCode(code, fileName,
                    parserImpl);

            compileResult.Program = program;


            var modules = program.getModules();
            
            for (int i = 0; i < modules.size(); i++)
            {
                var module = (ModuleImpl)modules.get(i);
                var ctx = module.getCtx();
                var modName = module.getName();
                compileResult.ModuleNames.Add(modName);
                // TODO: use Trivia Syntax Elements?
                compileResult.CSharpCode += "// Module Name: " + modName + "\r\n";
                if (translate)
                {
                    var formTree = new TranslatorForForm(compileResult);
                    compileResult.CSharpCode += formTree.Translate().ToFullString() + "\r\n";

                    var syntaxTree = new Translator(compileResult).Translate(module);
                    compileResult.CSharpCode += syntaxTree.ToFullString() + "\r\n";
                }
                //syntaxTree.
                //visitor.visit(((ModuleImpl)modules.get(i)).getCtx());
            }

            return compileResult;
        }
    }

}
