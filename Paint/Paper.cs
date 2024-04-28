using BaseShapes;
using System.Windows.Controls;

namespace Paint
{
    public class Paper
    {
        private Canvas _content;
        private List<BaseShape> drawnShapes = new List<BaseShape>();

        public Canvas Content { get { return _content; } }

        public Paper()
        {
            _content = new Canvas();
        }

        public void AddShape(BaseShape prototype)
        {
            BaseShape shape = (BaseShape)prototype.Clone();
            drawnShapes.Add(shape);
            _content.Children.Add(shape.Render());
            Canvas.SetTop(shape.content, shape.Start.Y);
            Canvas.SetLeft(shape.content, shape.Start.X);
        }
    }
}
