using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class VB6NodeTree
    {

        private Dictionary<ParseTree, List<ParseTree>> children;
        private Dictionary<ParseTree, int> depths;
        private Dictionary<ParseTree, ImmutableList<IndexedPath>> paths;

        public VisitorCallback Init()
        {
            children = new Dictionary<ParseTree, List<ParseTree>>();
            depths = new Dictionary<ParseTree, int>();
            paths = new Dictionary<ParseTree, ImmutableList<IndexedPath>>();

            return new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    // Just make sure all nodes, even leaves, have empty children lists
                    if (!children.ContainsKey(node))
                    {
                        children[node] = new List<ParseTree>();
                    }

                    // Add this to children of parent
                    if (!children.ContainsKey(node.getParent()))
                    {
                        children[node.getParent()] = new List<ParseTree>();

                    }
                    children[node.getParent()].Add(node);

                    // Depth updating
                    if (!depths.ContainsKey(node))
                    {
                        depths[node] = 0;
                    }

                    // Add this to children of parent
                    if (!depths.ContainsKey(node.getParent()))
                    {
                        depths[node.getParent()] = 0;
                    }
                    depths[node] = depths[node.getParent()] + 1;

                    // Path updating
                    if (!paths.ContainsKey(node))
                    {
                        paths[node] = ImmutableList.Create<IndexedPath>();
                    }

                    // Add this to children of parent
                    if (!paths.ContainsKey(node.getParent()))
                    {
                        paths[node.getParent()] = ImmutableList.Create<IndexedPath>();
                    }
                    int childIndex = children[node.getParent()].Count;
                    string token = new String(node.getText().Take(50).ToArray());
                    if (token.Length >= 50)
                    {
                        token += "...";
                    }
                    paths[node] = paths[node.getParent()].Add(new IndexedPath(VbToCsharpPattern.LookupNodeType(node), childIndex, token));
                }
            };
        }

        public VB6NodeTree(CompileResult compileResult)
        {
            var visitorCallback = Init();
            VB6Compiler.Visit(compileResult, visitorCallback);
        }
        
        public VB6NodeTree(ParseTree pnode)
        {
            var visitorCallback = Init();
            var visitor = new VB6ASTTreeViewGeneratorVisitor(visitorCallback);
            visitor.visit(pnode);
        }

        public IEnumerable<ParseTree> GetAllNodes()
        {
            foreach (var key in children.Keys)
            {
                yield return key;
            }
        }

        public ParseTree GetRoot()
        {
            return children.Keys.First();
        }

        public ImmutableList<IndexedPath> GetPath(ParseTree node)
        {
            if (paths.ContainsKey(node))
            {
                return paths[node];
            }

            throw new InvalidOperationException("No such node.");
        }

        public int GetDepth(ParseTree node)
        {
            if (depths.ContainsKey(node))
            {
                return depths[node];
            }

            throw new InvalidOperationException("No such node.");
        }

        public List<ParseTree> GetChildren(ParseTree node)
        {
            if (children.ContainsKey(node))
            {
                return children[node];
            }

            if (node == null)
            {
                throw new InvalidOperationException("Null node.");
            }

            throw new InvalidOperationException("No such node: " + node.GetHashCode());
        }
    }
}
