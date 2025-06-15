using headspace.Models.Common;
using Microsoft.UI.Xaml;

namespace headspace
{
    public partial class App : Application
    {
        public Window m_window;
        public Project CurrentProject { get; set; }

        public App()
        {
            this.InitializeComponent();

            CurrentProject = new Project();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }
    }
}
