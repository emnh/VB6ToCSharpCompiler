using org.antlr.v4.runtime.tree;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VB6ToCSharpCompiler
{
    public class VB6SubTree
    {
        ParseTree root;
        public VB6NodeTree parent { get; set; }
        int depth = 0;

        public VB6SubTree(VB6NodeTree parent, ParseTree root)
        {
            this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.root = root;
            depth = parent.GetDepth(root);
        }

        public ImmutableList<IndexedPath> GetPath(ParseTree node)
        {
            return parent.GetPath(node).Skip(depth).ToImmutableList();
        }

        public IEnumerable<ParseTree> GCRec(ParseTree node)
        {
            if (parent.GetChildren(node).Count > 0)
            {
                foreach (var child in parent.GetChildren(node))
                {
                    yield return child;
                    foreach (var child2 in GCRec(child))
                    {
                        yield return child;
                    }
                }
            }
            else
            {
                yield return node;
            }
        }

        public ParseTree GetRoot()
        {
            return root;
        }

        public IEnumerable<ParseTree> GetAllNodes()
        {
            yield return root;
            foreach (var child in GCRec(root))
            {
                yield return child;
            }
        }

    }
}
