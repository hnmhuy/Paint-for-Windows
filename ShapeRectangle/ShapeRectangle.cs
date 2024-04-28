using BaseShapes;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ShapeRectangle
{
    public class ShapeRectangle : BaseShape
    {
        private Rectangle rectangle;

        public ShapeRectangle()
        {
            _name = "Rectangle";
            _iconName = String.Empty;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override Canvas Render()
        {
            _canvas = new Canvas();
            rectangle = new Rectangle()
            {
                Width = Math.Abs(_start.X - _end.X),
                Height = Math.Abs(_end.Y - _start.Y),
                StrokeThickness = strokeThickness,
                Stroke = this._colorStroke,
                Fill = this._colorFill,
            };
            this._canvas.Children.Add(rectangle);
            return _canvas;

        }

        public override void Resize()
        {
            if (rectangle != null)
            {
                rectangle.Width = Math.Abs(_start.X - _end.X);
                rectangle.Height = Math.Abs(_start.Y - _end.Y);
            }
        }
    }
}
