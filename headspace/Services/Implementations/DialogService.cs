using headspace.Services.Interfaces;
using headspace.Views.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace headspace.Services.Implementations
{
    public class DialogService : IDialogService
    {
        public async Task<string?> ShowRenameDialogAsync(string currentName, XamlRoot xamlRoot)
        {
            var dialog = new RenameDialog(currentName)
            {
                XamlRoot = xamlRoot,
                Title = "Rename Item",
                PrimaryButtonText = "Rename",
                CloseButtonText = "Cancel"
            };

            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                return dialog.NewName;
            }

            return null;
        }
    }
}
