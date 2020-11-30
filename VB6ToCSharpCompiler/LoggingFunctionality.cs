using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
    public class LoggingFunctionality
    {
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
                yield return new OutToken(index + 1, "\r\n'PRECALL\r\n");
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
                //DebugClass.LogError("INDEX: " + index);
                yield return new OutToken(index + 1, "\r\n'POSTCALL\r\n");
            }
            else
            {
                DebugClass.LogError("POSTCALL FAIL");
            }
        }
    }
}
