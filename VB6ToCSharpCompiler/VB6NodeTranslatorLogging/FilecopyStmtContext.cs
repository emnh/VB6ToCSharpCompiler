
using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler.VB6NodeTranslatorLogging
{
public class FilecopyStmtContext : VB6NodeTranslator
{
    public FilecopyStmtContext(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict) : base(nodeTree, translatorDict)
    {
    }

    public override ContextNodeType GetNodeContextType()
    {
        return ContextNodeType.FilecopyStmtContext;
    }

    public override IEnumerable<OutToken> PreTranslate(List<ParseTree> parseTrees)
    {
        return new List<OutToken>();
    }
        
    public override IEnumerable<OutToken> PostTranslate(List<ParseTree> parseTrees)
    {
        return new List<OutToken>();
    }
}
}
