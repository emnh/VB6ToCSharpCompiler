using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VB6ToCSharpCompiler;

namespace ConsoleCompiler
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('c', "compile", Required = false, HelpText = "Compile files.")]
            public bool Compile { get; set; }

            [Option]
            public IEnumerable<string> Files { get; set; }
        }

        public static void Compile(IEnumerable<string> Files)
        {
            foreach (var fname in Files)
            {
                DebugClass.LogStandard("Parsing file: " + fname);

                var compileResult = VB6Compiler.Compile(fname);

                DebugClass.LogStandard(compileResult.CSharpCode);
            }
        }

        static void Main(string[] args)
        {
            TranslatorForPattern.IntializeTranslatorForPattern();

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       DebugClass.Init();

                       if (o.Verbose)
                       {
                           DebugClass.Enabled = true;
                       }

                       if (o.Compile)
                       {
                           //using (new OutputSink())
                           {
                               Compile(o.Files);
                           }
                           
                       }
                   });
        }
    }
}
