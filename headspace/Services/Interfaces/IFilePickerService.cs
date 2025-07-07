using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface IFilePickerService
    {
        Task<string?> PickSaveFileAsync();
        Task<string?> PickOpenFileAsync();
    }
}
