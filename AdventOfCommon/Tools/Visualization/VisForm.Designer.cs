namespace AdventOfCode.Tools.Visualization
{
    partial class VisForm
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
            this.visualRender = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.visualRender)).BeginInit();
            this.SuspendLayout();
            // 
            // visualRender
            // 
            this.visualRender.Location = new System.Drawing.Point(0, 0);
            this.visualRender.Name = "visualRender";
            this.visualRender.Size = new System.Drawing.Size(100, 50);
            this.visualRender.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.visualRender.TabIndex = 2;
            this.visualRender.TabStop = false;
            this.visualRender.Click += new System.EventHandler(this.VisForm_Click);
            // 
            // VisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(284, 239);
            this.Controls.Add(this.visualRender);
            this.Name = "VisForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Click += new System.EventHandler(this.VisForm_Click);
            ((System.ComponentModel.ISupportInitialize)(this.visualRender)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox visualRender;
    }
}