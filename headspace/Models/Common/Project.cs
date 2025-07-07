using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace headspace.Models.Common
{
    public partial class Project : ObservableObject
    {
        [ObservableProperty]
        private string projectName;

        public bool IsDirty =>
            Notes.Any(n => n.IsDirty) ||
            Documents.Any(d => d.IsDirty) ||
            Screenplays.Any(s => s.IsDirty) ||
            Drawings.Any(d => d.IsDirty) ||
            Moodboards.Any(m => m.IsDirty) ||
            Storyboards.Any(s => s.IsDirty) ||
            Musics.Any(m => m.IsDirty);

        public ObservableCollection<NoteModel> Notes { get; set; } = new ObservableCollection<NoteModel>();
        public ObservableCollection<DocumentModel> Documents { get; set; } = new ObservableCollection<DocumentModel>();
        public ObservableCollection<ScreenplayModel> Screenplays { get; set; } = new ObservableCollection<ScreenplayModel>();
        public ObservableCollection<DrawingModel> Drawings { get; set; } = new ObservableCollection<DrawingModel>();
        public ObservableCollection<MoodboardModel> Moodboards { get; set; } = new ObservableCollection<MoodboardModel>();
        public ObservableCollection<StoryboardModel> Storyboards { get; set; } = new ObservableCollection<StoryboardModel>();
        public ObservableCollection<MusicModel> Musics { get; set; } = new ObservableCollection<MusicModel>();
    }
}
