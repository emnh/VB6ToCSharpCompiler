﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.sun.org.apache.bcel.@internal.generic;
using com.sun.org.apache.xml.@internal.dtm.@ref;
using io.proleap.vb6.asg.metamodel;
using javax.swing.filechooser;
using jdk.nashorn.@internal.codegen;
using org.antlr.v4.runtime.tree;

namespace VB6ToCSharpCompiler
{
    public partial class frmVB6ASTBrowser : Form
    {
        private string FileName;
        private Translator translator;

        //private List<ParseTree> nodes = new List<ParseTree>();
        private Dictionary<ParseTree, TreeNode> nodeMap;

        public frmVB6ASTBrowser(string fileName)
        {
            this.FileName = fileName;
            InitializeComponent();
        }

        private void frmVB6ASTBrowser_Load(object sender, EventArgs e)
        {
            var compileResult = VB6Compiler.Compile(FileName, null, false);

            nodeMap = new Dictionary<ParseTree, TreeNode>();
            
            translator = new Translator(compileResult);

            var visitorCallback = new VisitorCallback()
            {
                Callback = (node, parent) =>
                {
                    var name = VbToCsharpPattern.LookupNodeType(node);
                    var lines = node.getText().Split('\n');
                    string firstLine = (lines.Length > 0) ? lines[0] : "";

                    if (parent != null && nodeMap.ContainsKey(parent))
                    {
                        var tvNode = nodeMap[parent].Nodes.Add(nodeMap.Count.ToString(new NumberFormatInfo()), name + ": " + firstLine);
                        tvNode.Tag = node;
                        nodeMap[node] = tvNode;
                    }
                    else
                    {
                        var tvNode = treVB6AST.Nodes.Add(nodeMap.Count.ToString(new NumberFormatInfo()), name + ": " + firstLine);
                        tvNode.Tag = node;
                        nodeMap[node] = tvNode;
                    }

                    //nodes.Add(node);
                }
            };

            txtDebug.ScrollBars = ScrollBars.Both;

            VB6Compiler.Visit(compileResult, visitorCallback);

            //treVB6AST.ExpandAll();
        }

        private void treVB6AST_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = (ParseTree) treVB6AST.SelectedNode.Tag;
            txtDebug.Text = translator.Dump(node) + "\r\n";

            //var apg = new ASTPatternGenerator((ParseTree) treVB6AST.Nodes[0].Tag);
            var asiList = ASTSequenceItem.Create(node);
            txtDebug.Text += ASTSequenceItem.ToString(asiList) + "\r\n";

        }

        private void txtDebug_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach (TreeNode node in nodeMap.Values)
            {
                //txtDebug.Text += node.Text;
                if (node.Text.Contains(txtSearch.Text))
                {
                    treVB6AST.SelectedNode = node;
                    node.EnsureVisible();
                    break;
                }
            }
            //txtDebug.Text = txtSearch.Text;
        }
    }
}
