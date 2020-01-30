using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using javax.swing.filechooser;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public partial class frmVB6ASTBrowser : Form
    {
        private string FileName;

        public frmVB6ASTBrowser(string fileName)
        {
            this.FileName = fileName;
            InitializeComponent();
        }

        private void frmVB6ASTBrowser_Load(object sender, EventArgs e)
        {
            var compileResult = VB6Compiler.Compile(FileName);

            var NodeMap = new Dictionary<ParseTree, TreeNode>();
            
            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    var name = node.GetType().Name;
                    if (parent != null && NodeMap.ContainsKey(parent))
                    {
                        var tvNode = NodeMap[parent].Nodes.Add(NodeMap.Count.ToString(new NumberFormatInfo()), name);
                        NodeMap[node] = tvNode;
                    }
                    else
                    {
                        var tvNode = treVB6AST.Nodes.Add(NodeMap.Count.ToString(new NumberFormatInfo()), name);
                        NodeMap[node] = tvNode;
                    }
                }
            };

            VB6Compiler.Visit(compileResult, visitorCallback);
        }

        private void treVB6AST_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
