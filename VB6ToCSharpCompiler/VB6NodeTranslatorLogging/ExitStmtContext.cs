
using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
public class ExitStmtContext : VB6NodeTranslator
{
    public ExitStmtContext(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict) : base(nodeTree, translatorDict)
    {
    }

    public override ContextNodeType GetNodeContextType()
    {
        return ContextNodeType.ExitStmtContext;
    }

    public override IEnumerable<string> PreTranslate(List<ParseTree> parseTrees)
    {
        return new List<string>();
    }
        
    public override IEnumerable<string> PostTranslate(List<ParseTree> parseTrees)
    {
        return new List<string>();
    }
}
}
