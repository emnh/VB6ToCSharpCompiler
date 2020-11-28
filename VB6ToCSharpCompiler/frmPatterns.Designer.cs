namespace VB6ToCSharpCompiler
{
    partial class frmPatterns
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
            this.grdPatterns = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.grdPatterns)).BeginInit();
            this.SuspendLayout();
            // 
            // grdPatterns
            // 
            this.grdPatterns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdPatterns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdPatterns.Location = new System.Drawing.Point(0, 0);
            this.grdPatterns.Name = "grdPatterns";
            this.grdPatterns.RowTemplate.Height = 24;
            this.grdPatterns.Size = new System.Drawing.Size(1068, 734);
            this.grdPatterns.TabIndex = 0;
            this.grdPatterns.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // frmPatterns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 734);
            this.Controls.Add(this.grdPatterns);
            this.Name = "frmPatterns";
            this.Text = "frmPatterns";
            this.Load += new System.EventHandler(this.frmPatterns_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdPatterns)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdPatterns;
    }
}