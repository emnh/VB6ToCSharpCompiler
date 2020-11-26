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

        public ParseTree GetRoot()
        {
            return children.Keys.First();
        }

        public List<ParseTree> GetChildren(ParseTree node)
        {
            if (children.ContainsKey(node))
            {
                return children[node];
            }

            throw new InvalidOperationException("No such node. Maybe wrong translator object for node?");
        }
    }
}
