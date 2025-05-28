using CommunityToolkit.Mvvm.ComponentModel;
using headspace.Models;
using Markdig;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace headspace.ViewModels
{
    public partial class NotesViewModel : ObservableObject
    {
        public ObservableCollection<Note> Notes { get; } = new();

        [ObservableProperty]
        private Note? selectedNote;

        [ObservableProperty]
        private string? markdownHtml;

        public NotesViewModel()
        {
            PropertyChanged += ViewModel_PropertyChanged;

            // Dummy Notes for now
            Notes.Add(new Note { Title = "Sample Note", Content = "# Hello, World\nThis is *preview*." });

            SelectedNote = Notes.FirstOrDefault();
            UpdatePreview();
        }

        partial void OnSelectedNoteChanged(Note? value)
        {
            UpdatePreview();
        }

        public void UpdatePreview()
        {
            if(SelectedNote is null)
            {
                MarkdownHtml = "";
                return;
            }

            string html = Markdown.ToHtml(SelectedNote.Content ?? "");
            MarkdownHtml = $@"
                <html>
                    <head>
                        <meta charset='utf-8' />
                    </head>
                    <body>
                        {html}
                    </body>
                </html>
            ";
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SelectedNote) && SelectedNote != null)
            {
                UpdatePreview();
            }
            else if(e.PropertyName == nameof(SelectedNote.Content))
            {
                UpdatePreview();
            }
        }
    }
}
