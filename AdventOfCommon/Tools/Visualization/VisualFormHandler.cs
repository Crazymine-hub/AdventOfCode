using System;
using System.Collections.Generic;
using Eto.Drawing;

namespace AdventOfCode.Tools.Visualization
{
    public class VisualFormHandler : IDisposable
    {

        private static readonly List<VisualFormHandler> instances = [];
        public static int ValidInstanceCount => instances.Count;

        public static VisualFormHandler GetInstance()
        {
            VisualFormHandler handler = new();
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

        private bool isDisposed;

        public string Title
        {
            get => visForm.Title;
            set => visForm.Title = value;
        }

        public bool Visible {
            get => visForm.Visible;
            set => visForm.Visible = value;
        }

        private VisualFormHandler()
        {
            visForm = VisForm.CreateInstance();
        }

        // [Obsolete($"Call {nameof(Update)} and set {nameof(Visible)} to true instead.")]
        // public void Show(Image? visualBmp = null, bool createCopy = true) =>
        //     throw new NotSupportedException("Method is deprecated");

        public void Reset()
        {
            visForm.Reset();
        }

        public void Invalidate()
        {
            try
            {
                visForm.Invalidate();
            }
            catch { }
        }

        public void Update(Image visualImage, bool createCopy = true)
        {
            visForm.DisplayImage?.Dispose();
            visForm.DisplayImage = createCopy ? new Bitmap(visualImage) : visualImage;
            Invalidate();
        }

        public void SetFocusTo(float x, float y)
        {
            visForm.FocusOnImage(x, y);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                visForm.Dispose();
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
