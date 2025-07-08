using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface IFilePickerService
    {
        Task<string?> PickSaveProjectAsync();
        Task<string?> PickOpenProjectAsync();
    }
}