using headspace.Utilities;
using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface ISettingsService
    {
        AppSettings CurrentSettings { get; }
        void ApplyTheme();
        Task SaveSettingsAsync();
    }
}
