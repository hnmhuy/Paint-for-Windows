using BaseShapes;
using System.Windows;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly BaseShape prototype = new ShapeRectangle.ShapeRectangle();
        private List<BaseShape> shapes = new List<BaseShape>();
        private bool isDrawing;
        Point start;
        Point end;
        UIElement drawingElement;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void paper_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //start = e.GetPosition(paper);
            //prototype.SetPosition(start, start);
            //drawingElement = prototype.Render();
            //paper.Children.Add(drawingElement);
            //isDrawing = true;
        }

        private void paper_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (isDrawing)
            //{
            //    end = e.GetPosition(paper);
            //    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            //    {
            //        prototype.SetProportionalPosition(start, end);
            //    }
            //    else
            //    {
            //        prototype.SetPosition(start, end);
            //    }
            //    prototype.Resize();
            //    Canvas.SetTop(drawingElement, prototype.Start.Y);
            //    Canvas.SetLeft(drawingElement, prototype.Start.X);

            //}
        }

        private void paper_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            shapes.Add((BaseShape)prototype.Clone());
            isDrawing = false;
        }
    }
}