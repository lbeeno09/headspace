using headspace.Services.Implementations;
using headspace.Services.Interfaces;
using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace headspace
{
    public partial class App : Application
    {
        public static Window MainWindow { get; private set; }
        private Window m_window;
        public IServiceProvider Services { get; }

        public App()
        {
            this.InitializeComponent();

            Services = ConfigureServices();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            MainWindow = m_window;
            m_window.Closed += OnMainWindowClosed;

            m_window.Activate();
        }

        private void OnMainWindowClosed(object sender, WindowEventArgs args)
        {
            var projectService = Services.GetRequiredService<IProjectService>() as ProjectService;
            projectService?.CleanupTemporaryProject();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IFilePickerService, FilePickerService>();
            services.AddSingleton<IProjectService, ProjectService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            // Components
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<NoteViewModel>();
            services.AddSingleton<DocumentViewModel>();
            services.AddSingleton<ScreenplayViewModel>();
            services.AddSingleton<DrawingViewModel>();
            services.AddSingleton<MoodboardViewModel>();
            services.AddSingleton<StoryboardViewModel>();
            services.AddSingleton<MusicViewModel>();
            services.AddSingleton<PreferenceViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
