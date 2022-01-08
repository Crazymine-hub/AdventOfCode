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

        private static List<VisualFormHandler> instances = new List<VisualFormHandler>();
        public static int ValidInstanceCount => instances.Count();

        private VisualFormHandler()
        {

        }

        public static VisualFormHandler GetInstance()
        {
            VisualFormHandler handler = new VisualFormHandler();
            instances.Add(handler);
            return handler;
        }

        public static void ClearAll()
        {
            foreach (VisualFormHandler handler in instances.ToArray())
                handler.Dispose();
            instances.Clear();
        }


        private VisForm visForm;
        private VisForm VisualForm
        {
            get
            {
                if (visForm != null) return visForm;
                visForm = VisForm.CreateInstance();
                VisualForm.Disposed += HandleDisposedWindow;
                return visForm;
            }
        }

        private bool isDisposed;

        public string Title { get => VisualForm.Title; set => VisualForm.Title = value; }

        public void Show(Image visualBmp, bool createCopy = true)
        {
            if (VisualForm.InvokeRequired)
            {
                VisualForm.Invoke(new Action<Image, bool>(Show), visualBmp, createCopy);
                return;
            }
            Reset();
            Update(visualBmp, createCopy);
            VisualForm.Show();
        }

        public void Hide()
        {
            if (VisualForm.InvokeRequired)
            {
                VisualForm.Invoke(new MethodInvoker(Hide));
                return;
            }
            VisualForm.Hide();
        }

        public void Reset()
        {
            if (VisualForm.InvokeRequired)
            {
                VisualForm.Invoke(new MethodInvoker(Reset));
                return;
            }
            VisualForm.Reset();
        }

        public void Invalidate()
        {
            if (VisualForm.InvokeRequired)
            {
                VisualForm.Invoke(new MethodInvoker(Invalidate));
                return;
            }
            try
            {
                VisualForm.Invalidate();
            }
            catch { }
        }

        public void Update(Image visualImage, bool createCopy = true)
        {
            if (VisualForm.InvokeRequired)
            {
                VisualForm.Invoke(new Action<Image, bool>(Update), visualImage, createCopy);
                return;
            }
            VisualForm.DisplayImage?.Dispose();
            VisualForm.DisplayImage = createCopy ? (Image)visualImage.Clone() : visualImage;
            Invalidate();
        }

        public void SetFocusTo(double x, double y)
        {
            if (VisualForm.InvokeRequired)
            {
                VisualForm.Invoke(new Action<double, double>(SetFocusTo), x, y);
                return;
            }
            VisualForm.FocusOnImage(x, y);
        }

        private bool GetVisibility()
        {
            if (VisualForm.InvokeRequired)
            {
                return (bool)VisualForm.Invoke(new Func<bool>(GetVisibility));
            }
            return VisualForm.Visible;
        }

        private void HandleDisposedWindow(object sender, EventArgs e)
        {
            if (sender != VisualForm) return;
            this.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (VisualForm.InvokeRequired)
                        VisualForm.Invoke(new Action(VisualForm.Dispose));
                    else
                        VisualForm.Dispose();
                }
                isDisposed = true;
                instances.Remove(this);
            }
        }

        ~VisualFormHandler()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
