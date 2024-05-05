using Paint.Commands;
using ShapeLine;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint
{
    public partial class MainWindow : Window
    {
        private PaintApplication application = new PaintApplication();
        private bool isFill = false;
        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = application;
            application.GenerateShapeControls(ShapeList);
            application.PropertyChanged += Application_PropertyChanged;
            application.FillColor = new SolidColorBrush(Colors.Transparent);
        }

        private void Application_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Check if the property changed is CurrentTool
            if (e.PropertyName == nameof(application.CurrentTool))
            {
                if (application.CurrentTool == ToolType.CopyToClipboard)
                {
                    CopyToClipboard.IsChecked = true;
                }
                else if (application.CurrentTool == ToolType.Select)
                {
                    SingleShapeSelector.IsChecked = true;
                }
                else
                {
                    SingleShapeSelector.IsChecked = false;
                    CopyToClipboard.IsChecked = false;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            application.DrawSpace = DrawSpace;
            DrawSpace.Children.Add(application.MainPage);
            application.LayerReview = LayerReviews;
        }

        private void DrawSpace_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (application.CurrentTool == ToolType.Draw)
            {
                application.StartDrawing(e.GetPosition(application.CurrentPage.Content));
            }
            else if (application.CurrentTool == ToolType.CopyToClipboard)
            {
                Canvas canvas = application.CurrentPage.Content;
                CopyToClipboardHandler.Instance.StartSelecting(e.GetPosition(canvas), canvas);
            }

        }

        private void DrawSpace_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (application.CurrentTool == ToolType.Draw)
            {
                // Checking if the shift button is pressing
                bool isShift = (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
                application.Drawing(e.GetPosition(application.CurrentPage.Content), isShift);
            }
            else if (application.CurrentTool == ToolType.CopyToClipboard)
            {
                CopyToClipboardHandler.Instance.UpdateSelecting(e.GetPosition(application.CurrentPage.Content));
            } else if (application.CurrentTool == ToolType.MovingShape)
            {
                application.OnMovingShape(e.GetPosition(application.CurrentPage.Content));
            }
        }

        private void DrawSpace_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (application.CurrentTool == ToolType.Draw)
            {
                application.DrawComplete();
            }
            else if (application.CurrentTool == ToolType.CopyToClipboard)
            {
                CopyToClipboardHandler.Instance.IsSelecting = false;
                CopyToClipboardHandler.Copy(application.MainPage);
            } else if (application.CurrentTool == ToolType.MovingShape)
            {
                application.OnMovingShapeComplete(e.GetPosition(application.MainPage));
            }
        }

        private void thicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            application.Thickness = Math.Round(e.NewValue);
        }
        private void DropDownButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == strokeTypeList)
            {
                // Show the context menu for strokeTypeList
                strokeTypeList.ContextMenu.IsOpen = true;
            }
            else if (sender == fillTypeList)
            {
                // Show the context menu for fillTypeList
                fillTypeList.ContextMenu.IsOpen = true;
            }
        }
        private void MenuStrokeItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedItem = sender as MenuItem;
            if (clickedItem != null)
            {
                // Retrieve the Tag property value, which contains the stroke dash array information
                string tagValue = clickedItem.Tag as string;
                // Set the content of the DropDownButton to a Canvas
                SetCanvasContent(tagValue);
            }
        }

        private void SetCanvasContent(string tagValue)
        {
            // Create a Canvas with a Line based on the tagValue
            Canvas canvas = new Canvas();
            canvas.Height = 20;
            canvas.Width = 70;
            Line line = new Line()
            {
                X1 = 0,
                X2 = 70,
                Y1 = 10,
                Y2 = 10,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };

            // Set the StrokeDashArray property based on tagValue
            switch (tagValue)
            {
                case "Custom 0":
                    line.StrokeDashArray = new DoubleCollection() { };
                    break;
                case "Custom 1":
                    line.StrokeDashArray = new DoubleCollection() { 1 };
                    break;
                case "Custom 2":
                    line.StrokeDashArray = new DoubleCollection() { 1, 3 };
                    break;
                case "Custom 3":
                    line.StrokeDashArray = new DoubleCollection() { 4, 1 };
                    break;
                case "Custom 4":
                    line.StrokeDashArray = new DoubleCollection() { 4, 3 };
                    break;
                case "Custom 5":
                    line.StrokeDashArray = new DoubleCollection() { 5, 2, 2, 2 };
                    break;
                case "Custom 6":
                    line.StrokeDashArray = new DoubleCollection() { 5, 2, 1, 1, 1, 2 };
                    break;
                default:
                    break;
            }

            application.StrokeType = line.StrokeDashArray;

            canvas.Children.Add(line);
            strokeTypeList.Content = canvas;
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Undo");
            application.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Redo");
            application.Redo();
        }

        private void OutlineColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                application.StrokeColor = new SolidColorBrush(e.NewValue.Value);
                application.TextColor = new SolidColorBrush(e.NewValue.Value);
                Debug.WriteLine("Outline color: " + e.NewValue.Value);

            }
        }

        private void FillColorPicker1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (isFill == true)
            {
                if (e.NewValue.HasValue)
                {
                    application.FillColor = new SolidColorBrush(e.NewValue.Value);
                    Debug.WriteLine("Fill color: " + e.NewValue.Value);
                }

            }
            else
            {
                application.FillColor = new SolidColorBrush(Colors.Transparent);
            }
            application.TextBackgroundColor = new SolidColorBrush(e.NewValue.Value);

        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            application.UnselectShape();
            if (((ToggleButton)sender).IsChecked == true)
            {
                application.CurrentTool = ToolType.CopyToClipboard;
            }
            else
            {
                application.CurrentTool = ToolType.None;
            }
        }

        private void MenuFillItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedItem = sender as MenuItem;
            if (clickedItem != null)
            {
                string seletedFill = (string)clickedItem.Header;
                fillTypeList.Content = seletedFill;
                switch (seletedFill)
                {
                    case "No fill":
                        isFill = false;
                        application.FillColor = new SolidColorBrush(Colors.Transparent);
                        break;
                    case "Solid fill":
                        isFill = true;
                        application.FillColor = new SolidColorBrush(FillColorPicker.SelectedColor.Value);
                        break;
                    default:
                        isFill = false;
                        application.FillColor = new SolidColorBrush(Colors.Transparent);
                        break;

                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            application.SaveFile();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            application.OpenFile();
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (!application.IsEmpty())
            {
                // Ask for comfirmation
                MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    application.NewFile();
                }
            }
            else return;
        }

        private void DrawSpace_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change the cursor depending on the current tool
            ToolType currentTool = application.CurrentTool;
            if (currentTool == ToolType.None)
            {
                Cursor = Cursors.Arrow;
            }
            else if (currentTool == ToolType.Draw)
            {
                Cursor = Cursors.Pen;
            }
            else if (currentTool == ToolType.CopyToClipboard)
            {
                Cursor = Cursors.Cross;
            }
        }

        private void DrawSpace_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void SingleShapeSelector_Click(object sender, RoutedEventArgs e)
        {
            application.UnselectShape();
            CopyToClipboard.IsChecked = false;
            application.ChangeToSelectingMode((bool)SingleShapeSelector.IsChecked);

            
        }

    }
}