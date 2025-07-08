using headspace.Models.Common;
using headspace.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace headspace.Views
{
    public sealed partial class StoryboardPage : Page
    {
        public StoryboardViewModel ViewModel { get; }

        private readonly List<Point> _currentPoints = new();
        private bool _isDrawing = false;
        private Color _activeColor;

        public StoryboardPage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<StoryboardViewModel>();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName is nameof(ViewModel.SelectedItem) or nameof(ViewModel.ActivePanel))
            {
                StoryboardCanvas.Invalidate();
            }
        }

        private void StoryboardCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if(ViewModel.SelectedItem == null || ViewModel.ActivePanel == null)
            {
                return;
            }

            int activePanelIndex = ViewModel.SelectedItem.Panels.IndexOf(ViewModel.ActivePanel);


            // 1. onion skinning
            if(activePanelIndex > 0)
            {
                var previousPanel = ViewModel.SelectedItem.Panels[activePanelIndex - 1];
                using(args.DrawingSession.CreateLayer(0.3f))
                {
                    foreach(var stroke in previousPanel.Strokes)
                    {
                        DrawStroke(sender, args, stroke);
                    }
                }
            }

            // 2. Draw already drawn lines
            foreach(var stroke in ViewModel.ActivePanel.Strokes)
            {
                DrawStroke(sender, args, stroke);
            }

            // 3. draw live strokes
            if(_isDrawing && _currentPoints.Count > 1)
            {
                var strokeWidth = ViewModel.IsEraserMode ? 20.0f : ViewModel.StrokeThickness;
                for(int i = 0; i < _currentPoints.Count - 1; i++)
                {
                    args.DrawingSession.DrawLine(
                        (float)_currentPoints[i].X, (float)_currentPoints[i].Y,
                        (float)_currentPoints[i + 1].X, (float)_currentPoints[i + 1].Y,
                        _activeColor, strokeWidth
                        );
                }
            }
        }

        private void DrawStroke(CanvasControl sender, CanvasDrawEventArgs args, StrokeData stroke)
        {
            if(stroke.CachedGeometry == null && stroke.Points.Count > 1)
            {
                using var pathBuilder = new CanvasPathBuilder(sender);
                pathBuilder.BeginFigure(stroke.Points[0]);
                for(int i = 1; i < stroke.Points.Count; i++)
                {
                    pathBuilder.AddLine(stroke.Points[i]);
                }
                pathBuilder.EndFigure(CanvasFigureLoop.Open);

                stroke.CachedGeometry = CanvasGeometry.CreatePath(pathBuilder);
            }

            if(stroke.CachedGeometry != null)
            {
                args.DrawingSession.DrawGeometry(stroke.CachedGeometry, stroke.Color, stroke.StrokeWidth);
            }
        }

        private void StoryboardCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(ViewModel.ActivePanel == null)
            {
                return;
            }

            var properties = e.GetCurrentPoint(StoryboardCanvas).Properties;
            if(properties.IsLeftButtonPressed)
            {
                _activeColor = ViewModel.IsEraserMode ? Colors.White : ViewModel.PrimaryColor;
            }
            else if(properties.IsRightButtonPressed)
            {
                _activeColor = ViewModel.IsEraserMode ? Colors.White : ViewModel.SecondaryColor;
            }
            else
            {
                return;
            }

            _isDrawing = true;
            _currentPoints.Clear();
            _currentPoints.Add(e.GetCurrentPoint(StoryboardCanvas).Position);
        }

        private void StoryboardCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(!_isDrawing)
            {
                return;
            }

            _currentPoints.Add(e.GetCurrentPoint(StoryboardCanvas).Position);

            StoryboardCanvas.Invalidate();
        }


        private void StoryboardCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if(!_isDrawing || _currentPoints.Count < 2 || ViewModel.ActivePanel == null)
            {
                return;
            }

            _isDrawing = false;


            var pointsToSave = _currentPoints.Select(p => new Vector2((float)p.X, (float)p.Y)).ToList();
            var stroke = new StrokeData
            {
                Points = pointsToSave,
                Color = _activeColor,
                StrokeWidth = ViewModel.IsEraserMode ? 20.0f : ViewModel.StrokeThickness
            };

            ViewModel.ActivePanel.Strokes.Add(stroke);

            _currentPoints.Clear();

            StoryboardCanvas.Invalidate();
        }
    }
}
