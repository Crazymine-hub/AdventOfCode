using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdventOfCode.Tools.Visualization
{
    public class VisualFormHandler : IDisposable
    {
        private static VisualFormHandler _instance;
        public static VisualFormHandler Instance => _instance ?? (_instance = new VisualFormHandler());


        private VisForm visForm = VisForm.Instance;
        private bool isDisposed;

        public bool Visible => GetVisibility();

        public void Show(Image visualBmp)
        {
            if (visForm.InvokeRequired)
            {
                visForm.Invoke(new Action<Image>(Show), visualBmp);
                return;
            }
            Reset();
            Update(visualBmp);
            visForm.Show();
        }

        public void Hide()
        {
            if (visForm.InvokeRequired)
            {
                visForm.Invoke(new MethodInvoker(Hide));
                return;
            }
            visForm.Hide();
        }

        public void Reset()
        {
            if (visForm.InvokeRequired)
            {
                visForm.Invoke(new MethodInvoker(Reset));
                return;
            }
            visForm.Reset();
        }

        public void Invalidate()
        {
            if (visForm.InvokeRequired)
            {
                visForm.Invoke(new MethodInvoker(Invalidate));
                return;
            }
            try
            {
                visForm.visualRender.Invalidate();
            }
            catch { }
        }

        public void Update(Image visualImage)
        {
            if (visForm.InvokeRequired)
            {
                visForm.Invoke(new Action<Image>(Update), visualImage);
                return;
            }
            visForm.visualRender.Image?.Dispose();
            visForm.visualRender.Image = visualImage;
            Invalidate();
        }

        public void SetFocusTo(double x, double y)
        {
            if (visForm.InvokeRequired)
            {
                visForm.Invoke(new Action<double, double>(SetFocusTo), x, y);
                return;
            }
            visForm.HorizontalScroll.Value = MathHelper.Clamp((int)((x - visForm.Width / 2) / visForm.visualRender.Width * visForm.HorizontalScroll.Maximum), visForm.HorizontalScroll.Minimum, visForm.HorizontalScroll.Maximum);
            visForm.VerticalScroll.Value = MathHelper.Clamp((int)((y - visForm.Height / 2)/ visForm.visualRender.Height * visForm.VerticalScroll.Maximum), visForm.VerticalScroll.Minimum, visForm.VerticalScroll.Maximum);
        }

        private bool GetVisibility()
        {
            if (visForm.InvokeRequired)
            {
                return (bool)visForm.Invoke(new Func<bool>(GetVisibility));
            }
            return visForm.Visible;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    visForm.Dispose();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                isDisposed = true;
            }
        }

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~VisualFormHandler()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
