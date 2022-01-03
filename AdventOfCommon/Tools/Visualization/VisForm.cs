using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdventOfCode.Tools.Visualization
{
    internal partial class VisForm : Form
    {
        VisualState state;

        private string titleText;

        internal string Title
        {
            get => titleText;
            set
            {
                titleText = value;
                RefreshDisplayedText();
            }
        }

        internal Image DisplayImage
        {
            get => visualRender.Image;
            set => visualRender.Image = value;
        }

        private VisForm()
        {
            InitializeComponent();
            Reset();
            Visible = false;
        }

        internal static VisForm CreateInstance()
        {
            VisForm instance = null;
            Task.Run(() =>
            {
                instance = new VisForm();
                using (ApplicationContext context = new ApplicationContext(instance))
                    Application.Run(context);
            });
            while (instance == null || !instance.InvokeRequired) { };
            return instance;
        }

        private void RefreshDisplayedText()
        {
            Text = $"{titleText} - {state}";
        }

        internal void Reset()
        {
            BackColor = Color.Black;
            visualRender.Image?.Dispose();
            visualRender.Image = null;
        }

        private void VisForm_Click(object sender, EventArgs e)
        {
            state += 1;
            if ((int)state >= Enum.GetValues(typeof(VisualState)).Length)
                state = 0;

            RefreshDisplayedText();

            switch (state)
            {
                case VisualState.normal:
                    visualRender.Dock = DockStyle.None;
                    visualRender.SizeMode = PictureBoxSizeMode.AutoSize;
                    visualRender.BackColor = Color.Black;
                    break;
                case VisualState.zoomed:
                    visualRender.Dock = DockStyle.Fill;
                    visualRender.SizeMode = PictureBoxSizeMode.Zoom;
                    visualRender.BackColor = Color.DimGray;
                    break;
            }
        }

        internal void FocusOnImage(double x, double y)
        {
            HorizontalScroll.Value = MathHelper.Clamp(
                (int)((x - Width / 2) / visualRender.Width * HorizontalScroll.Maximum),
                HorizontalScroll.Minimum,
                HorizontalScroll.Maximum);
            VerticalScroll.Value = MathHelper.Clamp(
                (int)((y - Height / 2) / visualRender.Height * VerticalScroll.Maximum),
                VerticalScroll.Minimum,
                VerticalScroll.Maximum);
        }
    }
}
