using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class AmbiguousIdentifierContext : VB6NodeTranslator
    {
        public AmbiguousIdentifierContext(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict) : base(nodeTree, translatorDict)
        {
        }

        public override ContextNodeType GetNodeContextType()
        {
            return ContextNodeType.AmbiguousIdentifierContext;
        }

        public override IEnumerable<string> Translate(List<ParseTree> parseTrees)
        {
            foreach (var child in UniversalTranslate(parseTrees))
            {
                yield return child;
            }
        }
    }
}
