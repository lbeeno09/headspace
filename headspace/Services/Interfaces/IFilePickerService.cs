using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface IFilePickerService
    {
        Task<string?> PickSaveProjectAsync();
        Task<string?> PickOpenProjectAsync();

        Task<string?> PickSaveFileAsync_Markdown(string selectedItem);
        Task<string?> PickSaveFileAsync_Rtf(string selectedItem);
        Task<string?> PickSaveFileAsync_Png(string selectedItem);
        Task<string?> PickSaveFileAsync_Svg(string selectedItem);
        Task<string?> PickSaveFileAsync_Pdf(string selectedItem);
    }
}