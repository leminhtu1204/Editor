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
using MikiEditorUI.BusinessObject;
using MikiEditorUI.ViewModel;

using newAdorner;

namespace MikiEditorUI.View
{
    using System.Globalization;

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
        //private Rectangle rect;
        private Label label;

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

        private Label InitLabel(double w, Double h, string id=null)
        {
            return new Label
            {
                FontSize = 25,
                BorderBrush = Brushes.RoyalBlue,
                BorderThickness = new Thickness(2, 2, 2, 2),
                Foreground = Brushes.RoyalBlue,
                Width = w,
                Height = h,
                Name = id
            };
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (double.IsNaN(canvas.Width) && double.IsNaN(canvas.Height))
            {
                return;
            }
            if (_isDrawing)
            {
                startPoint = e.GetPosition(canvas);
                label = InitLabel(0, 0);
                Canvas.SetLeft(label, startPoint.X);
                Canvas.SetTop(label, startPoint.X);
                label.Name = "A" + new Random().Next(50000);
                canvas.Children.Add(label);
                label.Content = canvas.Children.Count;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                if (e.LeftButton == MouseButtonState.Released || label == null)
                    return;

                var pos = e.GetPosition(canvas);

                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                label.Width = w;
                label.Height = h;

                Canvas.SetLeft(label, x);
                Canvas.SetTop(label, y);
            }
            else
            {
                Label_MouseMove(sender, e);
            }
        }

        private bool CheckAvailableRect(Label _label, MouseButtonEventArgs e)
        {
            if (_label == null || double.IsNaN(_label.Width) || _label.Width < 2 || double.IsNaN(_label.Height) || _label.Height < 2)
            {
                removeAdorner();
                canvas.Children.Remove(_label);
                return false;
            }
            return true;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!CheckAvailableRect(label, e))
            {
                return;
            }
            AddOrUpdateFrame(label);
            StopDragging();
            e.Handled = true;
            label = null;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            canvas.MouseLeave += Label_MouseLeave;
            canvas.PreviewMouseLeftButtonDown += myCanvas_PreviewMouseLeftButtonDown;
            canvas.PreviewMouseLeftButtonUp += DragFinishedMouseHandler;
        }

        void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        void Label_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed || label == null) && _isDown)
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

                label = e.Source as Label;

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
                    UpdateFrameIndex(canvas, int.Parse(label.Content.ToString()));
                    canvas.Children.Remove(label);
                    RemoveFrame(label);
                    label = null;
                    selectedElement = null;
                    _isDrawing = true;
                }
            }
        }

        private void AdornerLayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (label != null)
            {
                AddOrUpdateFrame(label);
            }
        }

        private void Page_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadingFrame();
        }

        private void ReloadingFrame()
        {
            var model = this.DataContext as ShellViewModel;
            canvas.Children.Clear();

            if (model.CurrentPage != null)
            {
                foreach (var frame in model.CurrentPage.Frames)
                {
                    label =
                        InitLabel(
                            Math.Abs(ToOriginal(frame.Coordinates.TopLeft, model.CurrentPage.Zoom).X -
                                     ToOriginal(frame.Coordinates.TopRight, model.CurrentPage.Zoom).X),
                            Math.Abs(ToOriginal(frame.Coordinates.TopLeft, model.CurrentPage.Zoom).Y -
                                     ToOriginal(frame.Coordinates.BottomLeft, model.CurrentPage.Zoom).Y), frame.Id);

                    Canvas.SetLeft(label, ToOriginal(frame.Coordinates.TopLeft, model.CurrentPage.Zoom).X);
                    Canvas.SetTop(label, ToOriginal(frame.Coordinates.TopLeft, model.CurrentPage.Zoom).Y);
                    canvas.Children.Add(label);
                    label.Content = canvas.Children.Count;
                }
            }

           model.NotifyZoom();
        }

        private FramePoint ToOriginal(FramePoint point, int _scale)
        {
            double x = point.X / _scale;
            double y = point.Y / _scale;
            return new FramePoint {X = x, Y = y};
        }

        // Handler for drag stopping on user choise
        private void DragFinishedMouseHandler(object sender, MouseButtonEventArgs e)
        {
            if (!CheckAvailableRect(label, e))
            {
                return;
            }
            AddOrUpdateFrame(label);
            StopDragging();
            e.Handled = true;
        }

        private void RemoveFrame(Label _label)
        {
            var model = this.DataContext as ShellViewModel;

            model.RemoveFrame(_label.Name);
        }

        private void AddOrUpdateFrame(Label _label)
        {
            var model = this.DataContext as ShellViewModel;

            var x = Canvas.GetLeft(_label);
            var y = Canvas.GetTop(_label);

            var w = _label.Width;
            var h = _label.Height;

            var topLeft = new Point() { X = x, Y = y };
            var topRight = new Point() { X = x + w, Y = y };
            var bottomLeft = new Point() { X = x, Y = y + h };
            var bottomRight = new Point() { X = x + h, Y = y + w };

            model.AddOrUpdateFrame(_label.Name, topLeft, topRight, bottomLeft, bottomRight, int.Parse(_label.Content.ToString()));
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateFrameIndex(Canvas _canvas, int deletedIndex)
        {
            if (_canvas.Children.Count == 0)
            {
                return;
            }

            foreach (Label item in _canvas.Children)
            {
                var currentIndex = int.Parse(item.Content.ToString());
                if (currentIndex > deletedIndex)
                {
                    item.Content = (currentIndex - 1).ToString();
                    AddOrUpdateFrame(item);
                }
            }
        }

        public void ZoomIn()
        {
            var model = this.DataContext as ShellViewModel;
            if (model.CurrentPage != null)
            {
                if (model.CurrentPage.Zoom == 1)
                {
                    return;
                }
                model.CurrentPage.Zoom = model.CurrentPage.Zoom - 1;
                ReloadingFrame();
            }

        }

        public void ZoomOut()
        {
            var model = this.DataContext as ShellViewModel;
            if (model.CurrentPage != null)
            {
                model.CurrentPage.Zoom = model.CurrentPage.Zoom + 1;
                ReloadingFrame();
            }
        }
    }
}
