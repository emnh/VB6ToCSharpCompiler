using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public class VB6ASTTreeViewGeneratorVisitor : io.proleap.vb6.VisualBasic6BaseVisitor
    {
        private VisitorCallback Callback;

        public VB6ASTTreeViewGeneratorVisitor(VisitorCallback callback)
        {
            this.Callback = callback;
        }

        public override object visitChildren(RuleNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            ParseTree parent = node.getParent();

            Callback.Callback(node, parent);

            return base.visitChildren(node);
        }
    }
}
