using headspace.Services.Interfaces;
using headspace.Utilities;
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
                XamlRoot = App.MainWindow.Content.XamlRoot,
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

        public async Task<ConfirmDialogResult> ShowConfirmUnsavedChangesDialogAsync()
        {
            var dialog = new ContentDialog
            {
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Title = "Unsaved Changes",
                Content = "You have unsaved changes. Do you want to save your project before proceeding?",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Don't Save",
                CloseButtonText = "Cancel"
            };

            var result = await dialog.ShowAsync();

            return result switch
            {
                ContentDialogResult.Primary => ConfirmDialogResult.Save,
                ContentDialogResult.Secondary => ConfirmDialogResult.Discard,
                _ => ConfirmDialogResult.Cancel,
            };
        }

    }
}
