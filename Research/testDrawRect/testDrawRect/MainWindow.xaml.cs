using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace testDrawRect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point startPoint;
        private Rectangle rect;
        // The part of the rectangle under the mouse.
        HitType MouseHitType = HitType.None;
        // True if a drag is in progress.
        private bool DragInProgress;

        // The drag's last point.
        private Point LastPoint;

        public MainWindow()
        {
            InitializeComponent();
        }

        // The part of the rectangle the mouse is over.
        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        // Set a mouse cursor appropriate for the current hit type.
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(Rectangle rect, Point point)
        {
            if (rect == null)
            {
                return HitType.Body;
            }
            double left = Canvas.GetLeft(rect);
            double top = Canvas.GetTop(rect);
            double right = left + rect.Width;
            double bottom = top + rect.Height;
          
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 5;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            else if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        //click mouse (before drawing)
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point hitTest = e.GetPosition(canvas);
            HitTestResult result = VisualTreeHelper.HitTest(canvas, hitTest);
            if (result.VisualHit is Rectangle)
            {
                var rectangle = result.VisualHit as Rectangle;

                RectOnMouseDown(rectangle);
                return;
            }

            startPoint = e.GetPosition(canvas);

            rect = new Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 2,
            };
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.X);
            canvas.Children.Add(rect);
        }

        //draw rectangle
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // buong chuot
            if (e.LeftButton == MouseButtonState.Released || rect == null)
            {
                Point hitTest = e.GetPosition(canvas);
                HitTestResult result = VisualTreeHelper.HitTest(canvas, hitTest);
                if (result.VisualHit is Rectangle)
                {
                    var rectangle = result.VisualHit as Rectangle;

                    RectOnMouseMove(rectangle);
                    return;
                }
                this.RectOnMouseMove(null);
                return;
            }

            //nhan chuot
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

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //rect = null;
            DragInProgress = false;
        }

        private void RectOnMouseMove(Rectangle rectangle1)
        {
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(rectangle1, Mouse.GetPosition(canvas));
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(canvas);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;

                // Get the rectangle's current position.
                double new_x = Canvas.GetLeft(rectangle1);
                double new_y = Canvas.GetTop(rectangle1);
                double new_width = rectangle1.Width;
                double new_height = rectangle1.Height;

                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;
                    case HitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        break;
                    case HitType.B:
                        new_height += offset_y;
                        break;
                    case HitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 0) && (new_height > 0))
                {
                    // Update the rectangle.
                    Canvas.SetLeft(rectangle1, new_x);
                    Canvas.SetTop(rectangle1, new_y);
                    rectangle1.Width = new_width;
                    rectangle1.Height = new_height;

                    // Save the mouse's new location.
                    LastPoint = point;
                }
            }
        }

        private void RectOnMouseDown(Rectangle rectangle)
        {
            MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas));
            SetMouseCursor();

            LastPoint = Mouse.GetPosition(canvas);
            DragInProgress = true;
        }
    }
}
