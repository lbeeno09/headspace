using System.Threading.Tasks;

namespace headspace.Services.Implementations
{
    public class AppSettings
    {
        public string DefaultAuthorName { get; set; } = "User";
        public int DefaultFontSize { get; set; } = 12;
    }

    public class SettingService
    {
        public AppSettings CurrentSettings { get; private set; }

        public SettingService()
        {
            CurrentSettings = LoadSettingsFromFile() ?? new AppSettings();
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
