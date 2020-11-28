using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CommandLine;

namespace VB6ToCSharpCompiler
{
    public class CommandLineOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('c', "compile", Required = false, HelpText = "Compile files.")]
        public bool Compile { get; set; }

        [Option('g', "gui", Required = false, HelpText = "Show GUI.")]
        public bool GUI { get; set; }

        [Option]
        public IEnumerable<string> Files { get; set; }
    }

    class Program
    {
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
            Console.Error.WriteLine("Use -g option to start GUI, or --help to show help for command line.");

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                   .WithParsed<CommandLineOptions>(o =>
                   {
                       DebugClass.Init();

                       if (o.Verbose || o.GUI)
                       {
                           DebugClass.Enabled = true;
                       }

                       if (o.Compile || o.GUI)
                       {
                           TranslatorForPattern.IntializeTranslatorForPattern();
                       }

                       if (o.Compile)
                       {
                           //DebugClass.LogStandard("")
                           DebugClass.LogStandard("Compile");
                           if (o.Files == null)
                           {
                               DebugClass.LogStandard("No files specified!");
                           }
                           else
                           {
                               Compile(o.Files);
                           }

                       }

                       if (o.GUI)
                       {
                           Application.EnableVisualStyles();
                           Application.SetCompatibleTextRenderingDefault(false);
                           var frm = new frmCompiler();
                           Application.Run(frm);
                           frm.Dispose();
                       }
                   });

            
        }
    }    
}
