using System;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;

//using GLib;

namespace AdventOfCode.Tools.Visualization
{
    internal partial class VisForm : Form
    {
        private string titleText = string.Empty;

        internal new string Title
        {
            get => titleText;
            set
            {
                titleText = value;
                RefreshDisplayedText();
            }
        }

        internal Image? DisplayImage
        {
            get => visualRender.Image;
            set => visualRender.Image = value;
        }

        private VisForm()
        {
            InitializeComponent();
            Reset();
            Visible = false;
            // TODO Maybe center on active screen
            // this.Location = new Eto.Drawing.Point();
            Invalidate();
        }

        internal static VisForm CreateInstance()
        {
            VisForm? instance = null;
            System.Threading.Tasks.Task.Run(() =>
            {
                instance = new VisForm();
                new Eto.Forms.Application().Run(instance);
            });
            while (instance == null) { }
            return instance;
        }

        private void RefreshDisplayedText()
        {
            this.Title = $"{titleText}";
        }

        internal void Reset()
        {
            BackgroundColor = Eto.Drawing.Colors.Black;
            visualRender.Image?.Dispose();
            visualRender.Image = null;
        }

        private void VisualRender_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            visualRender.ResetView();
        }

        private void VisualRender_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;
            e.Graphics.ImageInterpolation = ImageInterpolation.None;
            e.Graphics.DrawImage(visualRender.Image, Point.Empty);
        }

        internal void FocusOnImage(float x, float y)
        {
            visualRender.MovePixel(new PointF(x, y), (Point)visualRender.Size / 2);
        }
    }
}
