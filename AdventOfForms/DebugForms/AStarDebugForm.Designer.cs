namespace AdventOfCode.Tools.Visualization.DebugForm
{
    partial class AStarDebugForm
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
            this.components = new System.ComponentModel.Container();
            this.nodeGrid = new System.Windows.Forms.TableLayoutPanel();
            this.CellToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // nodeGrid
            // 
            this.nodeGrid.AutoSize = true;
            this.nodeGrid.BackColor = System.Drawing.Color.Black;
            this.nodeGrid.ColumnCount = 1;
            this.nodeGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.nodeGrid.Location = new System.Drawing.Point(0, 0);
            this.nodeGrid.Name = "nodeGrid";
            this.nodeGrid.RowCount = 1;
            this.nodeGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.nodeGrid.Size = new System.Drawing.Size(41, 41);
            this.nodeGrid.TabIndex = 0;
            // 
            // AStarDebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.nodeGrid);
            this.Name = "AStarDebugForm";
            this.Text = "AStarDebugForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel nodeGrid;
        private System.Windows.Forms.ToolTip CellToolTip;
    }
}