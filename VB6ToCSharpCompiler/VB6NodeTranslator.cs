using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public abstract class VB6NodeTranslator {
        public Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict { get;  }
        
        public VB6NodeTree nodeTree { get; }

        public abstract ContextNodeType GetNodeContextType();
        public abstract IEnumerable<string> Translate(List<ParseTree> parseTrees);

        public VB6NodeTranslator(VB6NodeTree nodeTree, Dictionary<ContextNodeType, VB6NodeTranslator> translatorDict)
        {
            this.nodeTree = nodeTree;
            this.translatorDict = translatorDict;
        }
        public IEnumerable<string> UniversalTranslate(List<ParseTree> parseTrees)
        {
            if (parseTrees == null)
            {
                throw new ArgumentNullException(nameof(parseTrees));
            }

            foreach (var node in parseTrees)
            {
                if (!Enum.TryParse(node.GetType().Name, out ContextNodeType contextNodeType))
                {
                    throw new ArgumentException("contextNodeType");
                }
                var translator = translatorDict[contextNodeType];
                foreach (var child in translator.Translate(nodeTree.GetChildren(node))) {
                    yield return child;
                };
            }
        }
    }
}
