using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using MikiEditorUI.ViewModel;

using newAdorner;

namespace MikiEditorUI.View
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        private AdornerLayer aLayer;

        private bool _isDrawing = true;
        private bool _isDown;
        private bool _isDragging;
        private bool selected = false;
        private UIElement selectedElement = null;

        private Point _startPoint;
        private double _originalLeft;
        private double _originalTop;
        private Point startPoint;
        private Rectangle rect;

        public ShellView()
        {
            InitializeComponent();
        }

        private void removeAdorner()
        {
            if (selected)
            {
                selected = false;
                _isDrawing = true;
                if (selectedElement != null)
                {
                    var adorners = aLayer.GetAdorners(selectedElement);
                    if (adorners != null && adorners.Any())
                    {
                        aLayer.Remove(aLayer.GetAdorners(selectedElement).First());
                    }

                    selectedElement = null;
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawing)
            {
                startPoint = e.GetPosition(canvas);

                rect = new Rectangle
                {
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };

                Canvas.SetLeft(rect, startPoint.X);
                Canvas.SetTop(rect, startPoint.X);
                rect.Name = "A" + new Random().Next(50000);
                canvas.Children.Add(rect);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                if (e.LeftButton == MouseButtonState.Released || rect == null)
                    return;

                var pos = e.GetPosition(canvas);

                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                rect.Width = w;
                rect.Height = h;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (rect == null || double.IsNaN(rect.Width) || rect.Width < 10 || double.IsNaN(rect.Height) || rect.Height < 10)
            {
                removeAdorner();
                return;
            }

            AddOrUpdateFrame(rect);
            StopDragging();
            e.Handled = true;

            rect = null;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            canvas.MouseMove += canvas_MouseMove;
            canvas.MouseLeave += canvas_MouseLeave;

            canvas.PreviewMouseLeftButtonDown += myCanvas_PreviewMouseLeftButtonDown;
            //canvas.PreviewMouseLeftButtonUp += DragFinishedMouseHandler;
        }

        void canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed || rect == null) && _isDown)
            {
                if ((_isDragging == false) &&
                    ((Math.Abs(e.GetPosition(canvas).X - _startPoint.X) >
                      SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(canvas).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    _isDragging = true;

                if (_isDragging)
                {
                    Point position = Mouse.GetPosition(canvas);
                    Canvas.SetTop(selectedElement, position.Y - (_startPoint.Y - _originalTop));
                    Canvas.SetLeft(selectedElement, position.X - (_startPoint.X - _originalLeft));
                    Cursor = Cursors.Hand;
                }
            }
        }

        // Method for stopping dragging
        private void StopDragging()
        {
            if (_isDown)
            {
                _isDown = false;
                _isDragging = false;
                _isDrawing = false;
                Cursor = Cursors.Arrow;
            }
        }

        // Handler for element selection on the canvas providing resizing adorner
        private void myCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Remove selection on clicking anywhere the window
            if (selected)
            {
                selected = false;
                _isDrawing = true;
                if (selectedElement != null)
                {
                    // Remove the adorner from the selected element
                    if (aLayer.GetAdorners(selectedElement) != null && aLayer.GetAdorners(selectedElement).Any())
                    {
                        aLayer.Remove(aLayer.GetAdorners(selectedElement)[0]);
                    }
                    selectedElement = null;
                }
            }

            // If any element except canvas is clicked, 
            // assign the selected element and add the adorner
            if (e.Source != canvas)
            {
                _isDown = true;
                _isDrawing = false;
                _startPoint = e.GetPosition(canvas);

                selectedElement = e.Source as UIElement;

                rect = e.Source as Rectangle;

                _originalLeft = Canvas.GetLeft(selectedElement);
                _originalTop = Canvas.GetTop(selectedElement);

                aLayer = AdornerLayer.GetAdornerLayer(selectedElement);
                aLayer.Add(new HelperAdorner(selectedElement));
                EventHelper.RemoveAllEvents(aLayer);
                aLayer.PreviewMouseLeftButtonUp += AdornerLayer_PreviewMouseLeftButtonUp;
                EventHelper.delegates.Add(AdornerLayer_PreviewMouseLeftButtonUp);
                selected = true;
                e.Handled = true;
                Cursor = Cursors.Hand;
            }
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (selectedElement != null)
                {
                    canvas.Children.Remove(rect);
                    rect = null;
                    selectedElement = null;
                    _isDrawing = true;
                }
            }
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AdornerLayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (rect != null)
            {
                AddOrUpdateFrame(rect);
            }
        }

        private void Page_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = this.DataContext as ShellViewModel;
            canvas.Children.Clear();

            if (model.CurrentPage != null)
            {
                foreach (var frame in model.CurrentPage.Frames)
                {
                    rect = new Rectangle
                    {
                        Stroke = Brushes.LightBlue,
                        StrokeThickness = 2,
                        Fill = Brushes.Transparent,
                        Width = Math.Abs(frame.Coordinates.TopLeft.X - frame.Coordinates.TopRight.X),
                        Height = Math.Abs(frame.Coordinates.TopLeft.Y - frame.Coordinates.BottomLeft.Y)
                    };

                    Canvas.SetLeft(rect, frame.Coordinates.TopLeft.X);
                    Canvas.SetTop(rect, frame.Coordinates.TopLeft.Y);
                    canvas.Children.Add(rect);
                }
            }
        }

        private void AddOrUpdateFrame(Rectangle rect)
        {
            var model = this.DataContext as ShellViewModel;

            var x = Canvas.GetLeft(rect);
            var y = Canvas.GetTop(rect);

            var w = rect.Width;
            var h = rect.Height;

            var topLeft = new Point() { X = x, Y = y };
            var topRight = new Point() { X = x + w, Y = y };
            var bottomLeft = new Point() { X = x, Y = y + h };
            var bottomRight = new Point() { X = x + h, Y = y + w };

            model.AddOrUpdateFrame(rect.Name, topLeft, topRight, bottomLeft, bottomRight);
        }
    }
}
