using System.Linq;
using AdventOfCode.Tools.Visualization.ComponentTag;
using Eto.Drawing;
using Eto.Forms;

namespace AdventOfCode.Tools.Visualization.DebugForm
{
    partial class AStarDebugForm
    {
        private TableLayout nodeGrid;

        private void InitializeComponent()
        {
            this.nodeGrid = new() { Style = FormStyle.GridStyle };
            this.SuspendLayout();
            // 
            // nodeGrid
            // 
            this.nodeGrid.Tag = new DefaultTab() { Name = "nodeGrid" };
            // Old Windows Size constraint
            // this.nodeGrid.ColumnCount = 1;
            // this.nodeGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            // this.nodeGrid.RowCount = 1;
            // this.nodeGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.nodeGrid.TabIndex = 0;
            // 
            // AStarDebugForm
            // 
            this.ClientSize = new(800, 450);
            this.Controls.Append(this.nodeGrid);
            this.Title = "AStarDebugForm";
            this.Tag = new DefaultTab() { Name = "AStarDebugForm" };
            this.ResumeLayout();
        }
    }
}