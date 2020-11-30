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
            frmPatternsForm?.Close();
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
            this.btnPatterns = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstFileNames
            // 
            this.lstFileNames.FormattingEnabled = true;
            this.lstFileNames.ItemHeight = 16;
            this.lstFileNames.Location = new System.Drawing.Point(8, 15);
            this.lstFileNames.Margin = new System.Windows.Forms.Padding(4);
            this.lstFileNames.Name = "lstFileNames";
            this.lstFileNames.Size = new System.Drawing.Size(352, 724);
            this.lstFileNames.TabIndex = 9;
            this.lstFileNames.SelectedIndexChanged += new System.EventHandler(this.lstFileNames_SelectedIndexChanged);
            // 
            // txtCSharpCode
            // 
            this.txtCSharpCode.Location = new System.Drawing.Point(1055, 15);
            this.txtCSharpCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtCSharpCode.Multiline = true;
            this.txtCSharpCode.Name = "txtCSharpCode";
            this.txtCSharpCode.Size = new System.Drawing.Size(667, 725);
            this.txtCSharpCode.TabIndex = 8;
            // 
            // txtVBCode
            // 
            this.txtVBCode.Location = new System.Drawing.Point(369, 15);
            this.txtVBCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtVBCode.Multiline = true;
            this.txtVBCode.Name = "txtVBCode";
            this.txtVBCode.Size = new System.Drawing.Size(676, 725);
            this.txtVBCode.TabIndex = 7;
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(189, 747);
            this.btnConvert.Margin = new System.Windows.Forms.Padding(4);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(171, 69);
            this.btnConvert.TabIndex = 6;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // btnBrowseVB6AST
            // 
            this.btnBrowseVB6AST.Location = new System.Drawing.Point(369, 747);
            this.btnBrowseVB6AST.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseVB6AST.Name = "btnBrowseVB6AST";
            this.btnBrowseVB6AST.Size = new System.Drawing.Size(173, 69);
            this.btnBrowseVB6AST.TabIndex = 10;
            this.btnBrowseVB6AST.Text = "Browse VB6 AST";
            this.btnBrowseVB6AST.UseVisualStyleBackColor = true;
            this.btnBrowseVB6AST.Click += new System.EventHandler(this.btnBrowseVB6AST_Click);
            // 
            // btnPatterns
            // 
            this.btnPatterns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(7)))), ((int)(((byte)(208)))));
            this.btnPatterns.Location = new System.Drawing.Point(550, 749);
            this.btnPatterns.Margin = new System.Windows.Forms.Padding(4);
            this.btnPatterns.Name = "btnPatterns";
            this.btnPatterns.Size = new System.Drawing.Size(171, 68);
            this.btnPatterns.TabIndex = 11;
            this.btnPatterns.Text = "Patterns (Patience..)";
            this.btnPatterns.UseVisualStyleBackColor = false;
            this.btnPatterns.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 747);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 70);
            this.button1.TabIndex = 12;
            this.button1.Text = "Open Folder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(7)))), ((int)(((byte)(208)))));
            this.btnGenerate.Location = new System.Drawing.Point(729, 749);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(171, 68);
            this.btnGenerate.TabIndex = 13;
            this.btnGenerate.Text = "Generate Classes";
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // frmCompiler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1752, 1116);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnPatterns);
            this.Controls.Add(this.btnBrowseVB6AST);
            this.Controls.Add(this.lstFileNames);
            this.Controls.Add(this.txtCSharpCode);
            this.Controls.Add(this.txtVBCode);
            this.Controls.Add(this.btnConvert);
            this.Margin = new System.Windows.Forms.Padding(4);
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
        private System.Windows.Forms.Button btnPatterns;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnGenerate;
    }
}

