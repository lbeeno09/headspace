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
    public partial class NoteViewModel : ObservableObject
    {
        public ObservableCollection<NoteItem> Notes { get; } = new();

        [ObservableProperty]
        private NoteItem selectedNote;

        [ObservableProperty]
        private string markdownHtml;

        public NoteViewModel()
        {
        }

        partial void OnSelectedNoteChanged(NoteItem value)
        {
            if(value != null)
            {
                MarkdownHtml = Markdig.Markdown.ToHtml(value.Content ?? "");
            }
        }

        public void UpdateMarkdown(string newText)
        {
            if(SelectedNote != null)
            {
                SelectedNote.Content = newText;
                MarkdownHtml = Markdig.Markdown.ToHtml(newText);
            }
        }

        [RelayCommand]
        private void AddNote()
        {
            string baseTitle = "Untitled";
            string newTitle = baseTitle;

            int i = 1;
            while(Notes.Select(n => n.Title).Contains(newTitle))
            {
                newTitle = $"{baseTitle}{i++}";
            }

            var newNote = new NoteItem { Title = newTitle, Content = "" };

            Notes.Add(newNote);
            SelectedNote = newNote;
        }

        [RelayCommand]
        private void DeleteNote()
        {
            if(SelectedNote != null)
            {
                Notes.Remove(SelectedNote);
            }
        }

        [RelayCommand]
        public async Task RenameNoteAsync(XamlRoot xamlRoot)
        {
            if(SelectedNote == null || xamlRoot == null)
            {
                return;
            }

            var inputDialog = new ContentDialog
            {
                Title = "Rename Note",
                Content = new TextBox
                {
                    Text = SelectedNote.Title,
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
                    SelectedNote.Title = newTitle;
                }
            }
        }
    }
}
