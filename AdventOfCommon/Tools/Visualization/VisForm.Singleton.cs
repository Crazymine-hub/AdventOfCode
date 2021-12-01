using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdventOfCode.Tools.Visualization
{
    partial class VisForm
    {
        private static VisForm _instance;
        public static VisForm Instance => _instance ?? CreateInstance();
        private VisForm()
        {
            InitializeComponent();
            FormClosing += VisualizationForm_FormClosing;
            Reset();
        }

        private static VisForm CreateInstance()
        {
            Task.Run(() =>
            {
                _instance = new VisForm();
                using (ApplicationContext context = new ApplicationContext(_instance))
                    Application.Run(context);
            });
            while (_instance == null || !_instance.InvokeRequired) { };
            return _instance;
        }
        private void VisualizationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
