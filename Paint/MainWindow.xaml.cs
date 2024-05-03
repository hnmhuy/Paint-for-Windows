using Paint.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
            application.PropertyChanged += Application_PropertyChanged;
        }

        private void Application_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Check if the property changed is CurrentTool
            if (e.PropertyName == nameof(application.CurrentTool))
            {
                if (application.CurrentTool == ToolType.CopyToClipboard)
                {
                    CopyToClipboard.IsChecked = true;
                } else
                {
                    CopyToClipboard.IsChecked = false;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawSpace.Children.Add(application.CurrentPage.Content);
        }

        private void DrawSpace_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(application.CurrentTool == ToolType.Draw)
            {
                application.StartDrawing(e.GetPosition(application.CurrentPage.Content));
            } else if (application.CurrentTool == ToolType.CopyToClipboard)
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
            } else if (application.CurrentTool == ToolType.CopyToClipboard)
            {
                CopyToClipboardHandler.Instance.UpdateSelecting(e.GetPosition(application.CurrentPage.Content));
            }   
        }

        private void DrawSpace_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (application.CurrentTool == ToolType.Draw)
            {
                application.DrawComplete();
            } else if (application.CurrentTool == ToolType.CopyToClipboard)
            {
                CopyToClipboardHandler.Instance.IsSelecting = false;
                CopyToClipboardHandler.Copy(application.CurrentPage.Content);
            }
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

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            application.UnselectShape();
            if (((ToggleButton)sender).IsChecked == true)
            {
                application.CurrentTool = ToolType.CopyToClipboard;
            }
        } 
    }
}