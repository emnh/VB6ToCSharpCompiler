
using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
public class FunctionStmtContext : VB6NodeTranslator
{
        public FunctionStmtContext(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict) : base(nodeTree, translatorDict)
        {
        }

        public override ContextNodeType GetNodeContextType()
        {
            return ContextNodeType.FunctionStmtContext;
        }

        public override IEnumerable<OutToken> PreTranslate(List<ParseTree> parseTrees)
        {
            return LoggingFunctionality.FunctionEnter(this, parseTrees);
        }

        public override IEnumerable<OutToken> PostTranslate(List<ParseTree> parseTrees)
        {
            return LoggingFunctionality.FunctionLeave(this, parseTrees);
        }
    }
}
