﻿namespace VB6ToCSharpCompiler
{
    partial class frmVB6ASTBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treVB6AST = new System.Windows.Forms.TreeView();
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // treVB6AST
            // 
            this.treVB6AST.Location = new System.Drawing.Point(16, 15);
            this.treVB6AST.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.treVB6AST.Name = "treVB6AST";
            this.treVB6AST.Size = new System.Drawing.Size(1047, 1123);
            this.treVB6AST.TabIndex = 0;
            this.treVB6AST.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treVB6AST_AfterSelect);
            // 
            // txtDebug
            // 
            this.txtDebug.Location = new System.Drawing.Point(1071, 15);
            this.txtDebug.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDebug.Multiline = true;
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(1554, 1123);
            this.txtDebug.TabIndex = 1;
            this.txtDebug.TextChanged += new System.EventHandler(this.txtDebug_TextChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(17, 1147);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(925, 22);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // frmVB6ASTBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2703, 1351);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.txtDebug);
            this.Controls.Add(this.treVB6AST);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmVB6ASTBrowser";
            this.Text = "frmVB6ASTBrowser";
            this.Load += new System.EventHandler(this.frmVB6ASTBrowser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treVB6AST;
        private System.Windows.Forms.TextBox txtDebug;
        private System.Windows.Forms.TextBox txtSearch;
    }
}