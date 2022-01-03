using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdventOfCode.Tools.Visualization.DebugForm
{
    public partial class AStarDebugForm : Form
    {
        private readonly IEnumerable<AStarNode> nodes;
        private List<Panel> nodePanels = new List<Panel>();

        public AStarDebugForm(IEnumerable<AStarNode> nodes)
        {
            InitializeComponent();
            this.nodes = nodes;

            int minX = nodes.Min(x => x.X);
            int minY = nodes.Min(x => x.Y);
            int maxX = nodes.Max(x => x.X);
            int maxY = nodes.Max(x => x.Y);
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            nodeGrid.RowCount = width + 1;
            nodeGrid.ColumnCount = width + 1;
            var colTemplate = nodeGrid.ColumnStyles[0];
            for (int x = 1; x < width + 1; ++x)
            {
                nodeGrid.ColumnStyles.Add(new ColumnStyle(colTemplate.SizeType, colTemplate.Width));
                Label label = new Label();
                label.Text = (x - 1).ToString();
                label.ForeColor = Color.White;
                nodeGrid.Controls.Add(label);
                label.Dock = DockStyle.Fill;
                nodeGrid.SetColumn(label, x);
                nodeGrid.SetRow(label, 0);

            }


            var rowTemplate = nodeGrid.RowStyles[0];
            for (int y = 1; y < height + 1; ++y)
            {
                nodeGrid.RowStyles.Add(new RowStyle(rowTemplate.SizeType, rowTemplate.Height));
                Label label = new Label();
                label.Text = (y - 1).ToString();
                label.ForeColor = Color.White;
                nodeGrid.Controls.Add(label);
                label.Dock = DockStyle.Fill;
                nodeGrid.SetRow(label, y);
                nodeGrid.SetColumn(label, 0);
            }

            foreach (var node in nodes)
            {
                Panel panel = new Panel();
                panel.BackColor = Color.White;
                panel.Width = 5;
                panel.Height = 5;
                CellToolTip.SetToolTip(panel, node.ToString());
                panel.Tag = node;
                panel.Click += Panel_Click;
                nodeGrid.SetColumn(panel, node.X + 1);
                nodeGrid.SetRow(panel, node.Y + 1);
                nodeGrid.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;
                nodePanels.Add(panel);
            }
        }

        private void Panel_Click(object sender, EventArgs e)
        {

            ResetView();
            AStarNode node = (sender as Panel)?.Tag as AStarNode;
            if (node == null) throw new InvalidOperationException("The clicked target doesn't have a node associated.");
            List<AStarNode> path = AStarPathfinder.GetPathToNode(node);
            foreach (AStarNode pathNode in path)
            {
                var panel = nodePanels.Single(x => x.Tag == pathNode);
                panel.BackColor = Color.Green;
            }
        }

        private void ResetView()
        {
            foreach (var panel in nodePanels)
            {
                panel.BackColor = Color.White;
                CellToolTip.SetToolTip(panel, panel.Tag.ToString());
            }
        }
    }
}
