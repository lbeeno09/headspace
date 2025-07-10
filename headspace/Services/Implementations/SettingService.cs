using headspace.Services.Interfaces;
using headspace.Utilities;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace headspace.Services.Implementations
{
    public class SettingService : ISettingsService
    {
        public AppSettings CurrentSettings { get; private set; }

        public SettingService()
        {
            CurrentSettings = LoadSettingsFromFile() ?? new AppSettings();
        }

        public void ApplyTheme()
        {
            if(App.MainWindow?.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = CurrentSettings.Theme switch
                {
                    AppTheme.Light => ElementTheme.Light,
                    AppTheme.Dark => ElementTheme.Dark,
                    _ => ElementTheme.Default,
                };
            }
        }

        public Task SaveSettingsAsync()
        {
            return Task.CompletedTask;
        }

        private AppSettings? LoadSettingsFromFile()
        {
            return null;
        }
    }
}
