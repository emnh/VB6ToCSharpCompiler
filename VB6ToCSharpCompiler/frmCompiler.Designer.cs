namespace VB6ToCSharpCompiler
{
    partial class frmCompiler
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
            frmVb6AstBrowser?.Close();
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
            this.lstFileNames = new System.Windows.Forms.ListBox();
            this.txtCSharpCode = new System.Windows.Forms.TextBox();
            this.txtVBCode = new System.Windows.Forms.TextBox();
            this.btnConvert = new System.Windows.Forms.Button();
            this.btnBrowseVB6AST = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstFileNames
            // 
            this.lstFileNames.FormattingEnabled = true;
            this.lstFileNames.Location = new System.Drawing.Point(6, 12);
            this.lstFileNames.Name = "lstFileNames";
            this.lstFileNames.Size = new System.Drawing.Size(265, 589);
            this.lstFileNames.TabIndex = 9;
            this.lstFileNames.SelectedIndexChanged += new System.EventHandler(this.lstFileNames_SelectedIndexChanged);
            // 
            // txtCSharpCode
            // 
            this.txtCSharpCode.Location = new System.Drawing.Point(791, 12);
            this.txtCSharpCode.Multiline = true;
            this.txtCSharpCode.Name = "txtCSharpCode";
            this.txtCSharpCode.Size = new System.Drawing.Size(501, 590);
            this.txtCSharpCode.TabIndex = 8;
            // 
            // txtVBCode
            // 
            this.txtVBCode.Location = new System.Drawing.Point(277, 12);
            this.txtVBCode.Multiline = true;
            this.txtVBCode.Name = "txtVBCode";
            this.txtVBCode.Size = new System.Drawing.Size(508, 590);
            this.txtVBCode.TabIndex = 7;
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(6, 608);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(128, 56);
            this.btnConvert.TabIndex = 6;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // btnBrowseVB6AST
            // 
            this.btnBrowseVB6AST.Location = new System.Drawing.Point(141, 608);
            this.btnBrowseVB6AST.Name = "btnBrowseVB6AST";
            this.btnBrowseVB6AST.Size = new System.Drawing.Size(130, 56);
            this.btnBrowseVB6AST.TabIndex = 10;
            this.btnBrowseVB6AST.Text = "Browse VB6 AST";
            this.btnBrowseVB6AST.UseVisualStyleBackColor = true;
            this.btnBrowseVB6AST.Click += new System.EventHandler(this.btnBrowseVB6AST_Click);
            // 
            // btnTest
            // 
            this.btnTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(7)))), ((int)(((byte)(208)))));
            this.btnTest.Location = new System.Drawing.Point(277, 609);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(128, 55);
            this.btnTest.TabIndex = 11;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // frmCompiler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1314, 907);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnBrowseVB6AST);
            this.Controls.Add(this.lstFileNames);
            this.Controls.Add(this.txtCSharpCode);
            this.Controls.Add(this.txtVBCode);
            this.Controls.Add(this.btnConvert);
            this.Name = "frmCompiler";
            this.Text = "VB6 to CSharp Compiler";
            this.Load += new System.EventHandler(this.frmCompiler_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstFileNames;
        private System.Windows.Forms.TextBox txtCSharpCode;
        private System.Windows.Forms.TextBox txtVBCode;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnBrowseVB6AST;
        private System.Windows.Forms.Button btnTest;
    }
}

