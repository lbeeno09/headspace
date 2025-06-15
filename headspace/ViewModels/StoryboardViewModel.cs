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
    public partial class StoryboardViewModel : ObservableObject
    {
        public ListItemManagerViewModel<StoryboardItem> StoryboardListManager { get; }

        public StoryboardItem SelectedStoryboard => StoryboardListManager.SelectedItem;

        [ObservableProperty]
        private SolidColorBrush primaryColor = new SolidColorBrush(Colors.Black);
        [ObservableProperty]
        private SolidColorBrush secondaryColor = new SolidColorBrush(Colors.White);
        [ObservableProperty]
        private double strokeThickness = 2.0;
        [ObservableProperty]
        private bool isEraserMode = false;

        public ICommand ClearCanvasCommand { get; }
        public XamlRoot PageXamlRoot
        {
            set
            {
                if(value != null)
                {
                    StoryboardListManager.XamlRoot = value;
                }
            }
        }

        public StoryboardViewModel()
        {
            StoryboardListManager = new ListItemManagerViewModel<StoryboardItem>((App.Current as App).CurrentProject.Storyboards);

            StoryboardListManager.SelectedItem = StoryboardListManager.Items.FirstOrDefault();
            StoryboardListManager.OnItemSelected += (sender, item) => OnPropertyChanged(nameof(SelectedStoryboard));

            ClearCanvasCommand = new RelayCommand(ClearCanvas);
        }

        private void ClearCanvas()
        {
            if(SelectedStoryboard != null)
            {
                SelectedStoryboard.Content = "";

                System.Diagnostics.Debug.WriteLine($"Cleared drawing for: {SelectedStoryboard.Title}");
            }
        }

        [RelayCommand]
        private void ToggleErase()
        {
            IsEraserMode = !IsEraserMode;
        }
    }
}
