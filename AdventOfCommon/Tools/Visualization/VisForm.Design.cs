using System;
using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Tools.Visualization.ComponentTag;
using AdventOfCode.Tools.Visualization.Controls;
using Eto.Drawing;
using Eto.Forms;

namespace AdventOfCode.Tools.Visualization;

partial class VisForm
{
    private MovableImageView visualRender;

    [MemberNotNull(nameof(visualRender))]
    private void InitializeComponent()
    {
        this.visualRender = new();
        ((System.ComponentModel.ISupportInitialize)(this.visualRender)).BeginInit();
        this.SuspendLayout();
        this.visualRender.Tag = new DefaultTab() { Name = "visualRender" };
        this.visualRender.Size = new Eto.Drawing.Size(100, 50);
        this.visualRender.TabIndex = 2;
        this.visualRender.MouseDoubleClick += VisualRender_MouseDoubleClick;
        this.visualRender.Paint += VisualRender_Paint;
        this.ClientSize = new Size(300, 300);
        this.MinimumSize = new Size(300, 300);
        this.Content = this.visualRender;
        this.Tag = new DefaultTab() { Name = "VisForm" };
        ((System.ComponentModel.ISupportInitialize)(this.visualRender)).EndInit();
        this.ResumeLayout();
    }
}