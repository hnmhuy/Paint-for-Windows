using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

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
    }
}