using AdventOfCode.Tools.Pathfinding;
using AdventOfCode.Tools.Pathfinding.AStar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Eto.Forms;

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

            foreach (var node in nodes)
            {
                Panel panel = new Panel(){ Style = FormStyle.PanelStyle };
                panel.Width = 5;
                panel.Height = 5;
                panel.ToolTip=node.ToString();
                panel.Tag = node;
                panel.MouseDown += Panel_Click;
                nodeGrid.Add(panel, node.X, node.Y);
                //nodeGrid.Controls.Append(panel);
                nodePanels.Add(panel);
            }
        }

        private void Panel_Click(object? sender, EventArgs e)
        {

            ResetView();
            AStarNode node = (sender as Panel)?.Tag as AStarNode ?? 
                throw new InvalidOperationException("The clicked target doesn't have a node associated.");
            List<AStarNode> path = AStarPathfinder.GetPathToNode(node);
            foreach (Panel panel in nodePanels.Where(p => path.Contains(p.Tag)))
            {
                panel.Style = FormStyle.PathStyle;
            }
        }

        private void ResetView()
        {
            foreach (var panel in nodePanels)
                panel.Style = FormStyle.PanelStyle;
        }
    }
}
