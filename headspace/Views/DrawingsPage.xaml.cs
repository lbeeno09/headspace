using headspace.Models;
using headspace.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI;
using Windows.System;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace headspace.Views
{
    public sealed partial class DrawingsPage : Page
    {
        private bool isDrawing = false;
        private PointerUpdateKind currentButton;
        private Polyline? currentStroke;
        private List<Point> currentStrokePoints = new();
        private Color currentColor;

        public DrawingsPage()
        {
            this.InitializeComponent();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            DrawingCanvas.Focus(FocusState.Programmatic);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedDrawing))
            {
                RedrawCanvas();
            }
        }

        private void RedrawCanvas()
        {
            DrawingCanvas.Children.Clear();
            if(ViewModel.SelectedDrawing == null)
            {
                return;
            }

            foreach(var stroke in ViewModel.SelectedDrawing.Strokes)
            {
                var polyline = new Polyline
                {
                    Stroke = new SolidColorBrush(stroke.Color),
                    StrokeThickness = stroke.Thickness
                };
                foreach(var pt in stroke.Points)
                {
                    polyline.Points.Add(pt);
                }
                DrawingCanvas.Children.Add(polyline);
            }
        }

        private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(ViewModel.SelectedDrawing == null)
            {
                return;
            }

            var point = e.GetCurrentPoint(DrawingCanvas);
            if(!isDrawing)
            {
                if(point.Properties.IsLeftButtonPressed)
                {
                    isDrawing = true;
                    currentButton = PointerUpdateKind.LeftButtonPressed;
                    currentColor = ViewModel.ColorOptions[ViewModel.SelectedPrimaryColor];
                }
                else if (point.Properties.IsRightButtonPressed)
                {
                    isDrawing = true;
                    currentButton = PointerUpdateKind.RightButtonPressed;
                    currentColor = ViewModel.ColorOptions[ViewModel.SelectedSecondaryColor];
                }

                if(isDrawing)
                {
                    currentStrokePoints.Clear();

                    currentStroke = new Polyline
                    {
                        Stroke = new SolidColorBrush(ViewModel.IsEraserMode ? Colors.White : currentColor),
                        StrokeThickness = ViewModel.SelectedThickness
                    };
                    var pos = e.GetCurrentPoint(DrawingCanvas).Position;
                    currentStrokePoints.Add(pos);
                    currentStroke.Points.Add(pos);

                    DrawingCanvas.Children.Add(currentStroke);
                }
            }
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(!isDrawing || currentStroke == null)
            {
                return;
            }

            var point = e.GetCurrentPoint(DrawingCanvas);
            bool shouldDraw = currentButton switch
            {
                PointerUpdateKind.LeftButtonPressed => point.Properties.IsLeftButtonPressed,
                PointerUpdateKind.RightButtonPressed => point.Properties.IsRightButtonPressed,
                _ => false
            };

            if(shouldDraw)
            {
                var pos = e.GetCurrentPoint(DrawingCanvas).Position;
                currentStrokePoints.Add(pos);
                currentStroke.Points.Add(pos);
            }
        }


        private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if(!isDrawing || ViewModel.SelectedDrawing == null)
            {
                return;
            }
            isDrawing = false;

            if(currentStrokePoints.Count >= 1)
            {
                var stroke = new StrokeData
                {
                    Points = new List<Point>(currentStrokePoints),
                    Color = ViewModel.IsEraserMode ? Colors.White : currentColor,
                    Thickness = ViewModel.SelectedThickness
                };
                ViewModel.SelectedDrawing.Strokes.Add(stroke);
            }

            currentStrokePoints.Clear();
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            if(DataContext is DrawingViewModel viewModel)
            {
                _ = viewModel.RenameDrawingAsync(this.XamlRoot);
            }
        }

        private async void DrawingCanvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.V && Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
            {
                //await PasteFromClipboardAsync();
            }
        }

        //private async Task PasteFromClipboardAsync()
        //{
        //}
    }
}
