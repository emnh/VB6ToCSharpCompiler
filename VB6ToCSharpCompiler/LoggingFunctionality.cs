using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
    public static class LoggingFunctionality
    {
        public static string GetFunctionName(VB6NodeTranslator translator, List<ParseTree> parseTrees)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }
            foreach (var child in parseTrees)
            {
                if (VB6NodeTranslator.GetNodeTypeName(child).Contains("Identifier"))
                {
                    return child.getText();
                }
            }
            return "UNKNOWN_FUNCTION";
        }

        public static IEnumerable<OutToken> FunctionEnter(VB6NodeTranslator translator, List<ParseTree> parseTrees)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }
            if (parseTrees.Count > 0)
            {
                var index = translator.GetFirstOrLastTokenIndex(parseTrees[0]);
                //DebugClass.LogError("INDEX: " + index);
                var functionName = GetFunctionName(translator, parseTrees);

                yield return new OutToken(index + 1, @"
FileNum = FreeFile
Open App.Path & ""\ProgramLog.txt"" For Append As FileNum
Print #FileNum, ""ENTER $FUNCTION""
Close FileNum
".Replace("$FUNCTION", functionName));
            }
            else
            {
                DebugClass.LogError("PRECALL FAIL");
            }
        }

        public static IEnumerable<OutToken> FunctionLeave(VB6NodeTranslator translator, List<ParseTree> parseTrees)
        {
            if (translator == null)
            {
                throw new ArgumentNullException(nameof(translator));
            }
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }
            if (parseTrees.Count > 0)
            {
                var index = translator.GetFirstOrLastTokenIndex(parseTrees[parseTrees.Count - 1], true);

                var functionName = GetFunctionName(translator, parseTrees);

                yield return new OutToken(index + 1, @"
FileNum = FreeFile
Open App.Path & ""\ProgramLog.txt"" For Append As FileNum
Print #FileNum, ""LEAVE $FUNCTION""
Close FileNum
".Replace("$FUNCTION", functionName));
            }
            else
            {
                DebugClass.LogError("POSTCALL FAIL");
            }
        }
    }
}
