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
    public sealed partial class MoodboardPage : Page
    {
        public MoodboardViewModel ViewModel { get; }

        private readonly List<Point> _currentPoints = new();
        private bool _isDrawing = false;
        private Color _activeColor;

        public MoodboardPage()
        {
            this.InitializeComponent();

            ViewModel = ((App)Application.Current).Services.GetRequiredService<MoodboardViewModel>();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.SelectedItem))
            {
                MoodboardCanvas.Invalidate();
            }
        }

        private void MoodboardCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            // 1. Draw already drawn lines
            if(ViewModel.SelectedItem != null)
            {
                foreach(var stroke in ViewModel.SelectedItem.Strokes)
                {
                    // if geometry is not cached, create from points
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
            }

            // 2. draw live strokes
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

        private void MoodboardCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(ViewModel.SelectedItem == null)
            {
                return;
            }

            var properties = e.GetCurrentPoint(MoodboardCanvas).Properties;
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
            _currentPoints.Add(e.GetCurrentPoint(MoodboardCanvas).Position);
        }

        private void MoodboardCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if(!_isDrawing)
            {
                return;
            }

            _currentPoints.Add(e.GetCurrentPoint(MoodboardCanvas).Position);

            MoodboardCanvas.Invalidate();
        }

        private void MoodboardCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if(!_isDrawing || _currentPoints.Count < 2 || ViewModel.SelectedItem == null)
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

            ViewModel.SelectedItem.Strokes.Add(stroke);

            _currentPoints.Clear();

            MoodboardCanvas.Invalidate();
        }
    }
}
