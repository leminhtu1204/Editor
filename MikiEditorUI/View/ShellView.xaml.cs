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
                rect.Name = "A" + new Random().Next(50000);

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawing)
            {
                rect = null;
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.MouseLeftButtonDown += Window1_MouseLeftButtonDown;
            this.MouseLeftButtonUp += DragFinishedMouseHandler;
            this.MouseMove += Window1_MouseMove;
            this.MouseLeave += Window1_MouseLeave;

            canvas.PreviewMouseLeftButtonDown += myCanvas_PreviewMouseLeftButtonDown;
            canvas.PreviewMouseLeftButtonUp += DragFinishedMouseHandler;
        }

        // Handler for drag stopping on leaving the window
        private void Window1_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        // Method for stopping dragging
        private void StopDragging()
        {
            if (_isDown)
            {
                _isDown = false;
                _isDragging = false;
                _isDrawing = false;
            }
        }

        // Hanler for providing drag operation with selected element
        private void Window1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
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
                }
            }
        }

        // Handler for clearing element selection, adorner removal
        private void Window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selected)
            {
                selected = false;
                if (selectedElement != null)
                {
                    aLayer.Remove(aLayer.GetAdorners(selectedElement)[0]);
                    selectedElement = null;
                }
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
                    aLayer.Remove(aLayer.GetAdorners(selectedElement)[0]);
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

        // Handler for drag stopping on user choise
        private void DragFinishedMouseHandler(object sender, MouseButtonEventArgs e)
        {
            rect = e.Source as Rectangle;

            if (rect != null)
            {
                AddOrUpdateFrame(rect);
            }

            StopDragging();
            e.Handled = true;
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
                        Width = 100,
                        Height = 100
                    };

                    Canvas.SetLeft(rect, 50);
                    Canvas.SetTop(rect, 50);
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
            var topRight = new Point() { X = x, Y = y + w };
            var bottomLeft = new Point() { X = x + h, Y = y };
            var bottomRight = new Point() { X = x + h, Y = y + w };

            model.AddOrUpdateFrame(rect.Name, topLeft, topRight, bottomLeft, bottomRight);
        }
    }
}
