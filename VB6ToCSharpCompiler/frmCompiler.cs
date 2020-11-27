﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VB6ToCSharpCompiler
{
    public partial class frmCompiler : Form
    {
        private frmVB6ASTBrowser frmVb6AstBrowser;

        public frmCompiler()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            var fileName = (string) lstFileNames.SelectedItem;


            var compileResult = VB6Compiler.Compile(fileName);
            txtVBCode.Text = compileResult.VBCode;
            txtCSharpCode.Text += "// C Sharp Code:\r\n" + compileResult.CSharpCode;
            txtVBCode.ScrollBars = ScrollBars.Both;
            txtCSharpCode.ScrollBars = ScrollBars.Both;
        }

        private void ShowFiles()
        {
            lstFileNames.Items.Clear();
            foreach (var fileName in VB6Compiler.GetFiles())
            {
                if (
                    fileName.EndsWith(".bas", System.StringComparison.InvariantCulture) ||
                    fileName.EndsWith(".frm", System.StringComparison.InvariantCulture) ||
                    fileName.EndsWith(".frx", System.StringComparison.InvariantCulture))
                {
                    lstFileNames.Items.Add(fileName);
                }
            }
            TranslatorForPattern.IntializeTranslatorForPattern();
        }

        private void frmCompiler_Load(object sender, EventArgs e)
        {
            ShowFiles();
        }

        private void btnBrowseVB6AST_Click(object sender, EventArgs e)
        {
            var fileName = (string)lstFileNames.SelectedItem;
            frmVb6AstBrowser = new frmVB6ASTBrowser(fileName);
            frmVb6AstBrowser.Visible = true;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var fileName = (string)lstFileNames.SelectedItem;
            
            //var tfp = new TranslatorForPattern();

        }

        private void lstFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    VB6Compiler.Folder = fbd.SelectedPath;
                }
            }
            ShowFiles();
        }
    }
}
