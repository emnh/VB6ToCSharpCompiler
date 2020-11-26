using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class VB6NodeTree
    {

        private readonly Dictionary<ParseTree, List<ParseTree>> children;
        private readonly Dictionary<ParseTree, int> depths;

        public VB6NodeTree(CompileResult compileResult)
        {
            children = new Dictionary<ParseTree, List<ParseTree>>();

            var visitorCallback = new VisitorCallback()
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
                }
            };
            VB6Compiler.Visit(compileResult, visitorCallback);
        }
        
        public VB6NodeTree(ParseTree pnode)
        {
            children = new Dictionary<ParseTree, List<ParseTree>>();
            depths = new Dictionary<ParseTree, int>();

            var visitorCallback = new VisitorCallback()
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
                }
            };

            var visitor = new VB6ASTTreeViewGeneratorVisitor(visitorCallback);
            visitor.visit(pnode);
        }

        public ParseTree GetRoot()
        {
            return children.Keys.First();
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
