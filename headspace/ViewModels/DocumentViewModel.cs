using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace headspace.ViewModels
{
    public partial class DocumentViewModel : ObservableObject
    {
        public ObservableCollection<DocumentItem> Documents { get; } = new();

        [ObservableProperty]
        private DocumentItem selectedDocument;

        public DocumentViewModel()
        {
        }

        [RelayCommand]
        private void AddDocument()
        {
            string baseTitle = "Untitled";
            string newTitle = baseTitle;

            int i = 1;
            while(Documents.Select(n => n.Title).Contains(newTitle))
            {
                newTitle = $"{baseTitle}{i++}";
            }

            var newDocument = new DocumentItem { Title = newTitle, Content = "" };

            Documents.Add(newDocument);
            SelectedDocument = newDocument;
        }

        [RelayCommand]
        private void DeleteDocument()
        {
            if(SelectedDocument != null)
            {
                Documents.Remove(SelectedDocument);
            }
        }

        [RelayCommand]
        public async Task RenameDocumentAsync(XamlRoot xamlRoot)
        {
            if(SelectedDocument == null || xamlRoot == null)
            {
                return;
            }

            var inputDialog = new ContentDialog
            {
                Title = "Rename Document",
                Content = new TextBox
                {
                    Text = SelectedDocument.Title,
                    AcceptsReturn = false,
                    PlaceholderText = "Enter new title"
                },
                PrimaryButtonText = "Rename",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await inputDialog.ShowAsync();

            if(result == ContentDialogResult.Primary && inputDialog.Content is TextBox textBox)
            {
                var newTitle = textBox.Text.Trim();

                if(!string.IsNullOrWhiteSpace(newTitle))
                {
                    SelectedDocument.Title = newTitle;
                }
            }
        }
    }
}
