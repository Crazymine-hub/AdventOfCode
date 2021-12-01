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
        bool isZoomed = false;

        internal void Reset()
        {
            BackColor = Color.Black;
            visualRender.Image?.Dispose();
            visualRender.Image = null;
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {
            isZoomed = !isZoomed;
            if (isZoomed)
            {
                visualRender.Dock = DockStyle.Fill;
                visualRender.SizeMode = PictureBoxSizeMode.Zoom;
                visualRender.BackColor = Color.DarkGray;
            }
            else
            {
                visualRender.Dock = DockStyle.None;
                visualRender.SizeMode = PictureBoxSizeMode.AutoSize;
                visualRender.BackColor = Color.Black;
            }
        }
    }
}
