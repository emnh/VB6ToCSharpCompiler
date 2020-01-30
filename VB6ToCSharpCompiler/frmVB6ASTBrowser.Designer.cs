namespace VB6ToCSharpCompiler
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
            this.SuspendLayout();
            // 
            // treVB6AST
            // 
            this.treVB6AST.Location = new System.Drawing.Point(12, 12);
            this.treVB6AST.Name = "treVB6AST";
            this.treVB6AST.Size = new System.Drawing.Size(401, 510);
            this.treVB6AST.TabIndex = 0;
            this.treVB6AST.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treVB6AST_AfterSelect);
            // 
            // frmVB6ASTBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 747);
            this.Controls.Add(this.treVB6AST);
            this.Name = "frmVB6ASTBrowser";
            this.Text = "frmVB6ASTBrowser";
            this.Load += new System.EventHandler(this.frmVB6ASTBrowser_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treVB6AST;
    }
}