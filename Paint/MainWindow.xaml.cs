using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PaintApplication application = new PaintApplication();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = application;
            application.GenerateShapeControls(ShapeList);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawSpace.Children.Add(application.CurrentPage.Content);
        }

        private void DrawSpace_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            application.StartDrawing(e.GetPosition(application.CurrentPage.Content));
        }

        private void DrawSpace_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Checking if the shift button is pressing
            bool isShift = (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
            application.Drawing(e.GetPosition(application.CurrentPage.Content), isShift);
        }

        private void DrawSpace_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            application.DrawComplete();
        }

        private void thicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            application.Thickness = Math.Round(e.NewValue);
        }
        private void DropDownButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ensure the left mouse button is clicked
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Show the context menu
                strokeTypeList.ContextMenu.IsOpen = true;
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
                    line.StrokeDashArray = new DoubleCollection() {};
                    break;
                case "Custom 1":
                    line.StrokeDashArray = new DoubleCollection() { 1};
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
            if(e.NewValue.HasValue)
            {
                application.StrokeColor = new SolidColorBrush(e.NewValue.Value);
                Debug.WriteLine("Outline color: " + e.NewValue.Value);

            }
        }

        private void FillColorPicker1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                Debug.WriteLine(e.NewValue.Value);
            }
        }
    }
}