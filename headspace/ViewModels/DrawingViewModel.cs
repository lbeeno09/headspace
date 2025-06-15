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
    public partial class DrawingViewModel : ObservableObject
    {
        public ListItemManagerViewModel<DrawingItem> DrawingListManager { get; }

        public DrawingItem SelectedDrawing => DrawingListManager.SelectedItem;

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
                    DrawingListManager.XamlRoot = value;
                }
            }
        }

        public DrawingViewModel()
        {
            DrawingListManager = new ListItemManagerViewModel<DrawingItem>((App.Current as App).CurrentProject.Drawings);

            DrawingListManager.SelectedItem = DrawingListManager.Items.FirstOrDefault();
            DrawingListManager.OnItemSelected += (sender, item) => OnPropertyChanged(nameof(SelectedDrawing));

            ClearCanvasCommand = new RelayCommand(ClearCanvas);
        }

        private void ClearCanvas()
        {
            if(SelectedDrawing != null)
            {
                SelectedDrawing.Content = "";

                System.Diagnostics.Debug.WriteLine($"Cleared drawing for: {SelectedDrawing.Title}");
            }
        }

        [RelayCommand]
        private void ToggleErase()
        {
            IsEraserMode = !IsEraserMode;
        }
    }
}
