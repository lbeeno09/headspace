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
            Documents.Any(n => n.IsDirty) ||
            Screenplays.Any(n => n.IsDirty) ||
            Drawings.Any(n => n.IsDirty) ||
            Moodboards.Any(n => n.IsDirty) ||
            Storyboards.Any(n => n.IsDirty) ||
            Musics.Any(n => n.IsDirty);

        public ObservableCollection<NoteModel> Notes { get; set; } = new ObservableCollection<NoteModel>();
        public ObservableCollection<DocumentModel> Documents { get; set; } = new ObservableCollection<DocumentModel>();
        public ObservableCollection<ScreenplayModel> Screenplays { get; set; } = new ObservableCollection<ScreenplayModel>();
        public ObservableCollection<DrawingModel> Drawings { get; set; } = new ObservableCollection<DrawingModel>();
        public ObservableCollection<MoodboardModel> Moodboards { get; set; } = new ObservableCollection<MoodboardModel>();
        public ObservableCollection<StoryboardModel> Storyboards { get; set; } = new ObservableCollection<StoryboardModel>();
        public ObservableCollection<MusicModel> Musics { get; set; } = new ObservableCollection<MusicModel>();
    }
}
