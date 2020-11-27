using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class DebugClass
    {
        public static bool Enabled = false;
        public static TextWriter console;
        public static TextWriter error;

        public static void Init()
        {
            console = Console.Out;
            error = Console.Error;
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }

        public static void LogError(string message)
        {
            if (Enabled)
            {
                error.WriteLine(message);
            }
        }

        public static void LogStandard(string message)
        {
            console.WriteLine(message);   
        }
    }
}
