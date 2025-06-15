using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using headspace.Models;
using headspace.ViewModels.Common;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Linq;
using System.Windows.Input;

namespace headspace.ViewModels
{
    public partial class MoodboardViewModel : ObservableObject
    {
        public ListItemManagerViewModel<MoodboardItem> MoodboardListManager { get; }

        public MoodboardItem SelectedMoodboard => MoodboardListManager.SelectedItem;

        [ObservableProperty]
        private SolidColorBrush primaryColor = new SolidColorBrush(Colors.Black);
        [ObservableProperty]
        private SolidColorBrush secondaryColor = new SolidColorBrush(Colors.White);
        [ObservableProperty]
        private double strokeThickness = 2.0;
        [ObservableProperty]
        private bool isEraserMode;

        public ICommand ClearCanvasCommand { get; }

        public XamlRoot PageXamlRoot
        {
            set
            {
                if(value != null)
                {
                    MoodboardListManager.XamlRoot = value;
                }
            }
        }

        public MoodboardViewModel()
        {
            MoodboardListManager = new ListItemManagerViewModel<MoodboardItem>((App.Current as App).CurrentProject.Moodboards);

            MoodboardListManager.SelectedItem = MoodboardListManager.Items.FirstOrDefault();
            MoodboardListManager.OnItemSelected += (sender, item) => OnPropertyChanged(nameof(SelectedMoodboard));

            ClearCanvasCommand = new RelayCommand(ClearCanvas);

        }

        private void ClearCanvas()
        {
            if(SelectedMoodboard != null)
            {
                SelectedMoodboard.Content = "";

                System.Diagnostics.Debug.WriteLine($"Cleared drawing for: {SelectedMoodboard.Title}");
            }
        }

        [RelayCommand]
        private void ToggleErase()
        {
            IsEraserMode = !IsEraserMode;
        }
    }
}
