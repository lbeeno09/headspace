using headspace.Utilities;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace headspace.Services.Interfaces
{
    public interface IDialogService
    {
        Task<string?> ShowRenameDialogAsync(string currentName, XamlRoot xamlRoot);
        Task<ConfirmDialogResult> ShowConfirmUnsavedChangesDialogAsync();
    }
}
