using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace headspace.Models.Common
{
    public partial class Project : ObservableObject
    {
        [ObservableProperty]
        private string projectName = "Untitled";

        public ObservableCollection<NoteItem> Notes { get; set; } = new ObservableCollection<NoteItem>();
        public ObservableCollection<DocumentItem> Documents { get; set; } = new ObservableCollection<DocumentItem>();
        public ObservableCollection<ScreenplayItem> Screenplays { get; set; } = new ObservableCollection<ScreenplayItem>();
        public ObservableCollection<DrawingItem> Drawings { get; set; } = new ObservableCollection<DrawingItem>();
        public ObservableCollection<MoodboardItem> Moodboards { get; set; } = new ObservableCollection<MoodboardItem>();
        public ObservableCollection<StoryboardItem> Storyboards { get; set; } = new ObservableCollection<StoryboardItem>();
        public ObservableCollection<MusicItem> Musics { get; set; } = new ObservableCollection<MusicItem>();
    }
}
