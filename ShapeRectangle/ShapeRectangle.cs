using BaseShapes;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeRectangle
{
    public class ShapeRectangle : BaseShape
    {
        private Rectangle rectangle;

        public ShapeRectangle()
        {
            
            _name = nameof(ShapeRectangle);
            _iconName = "rectangle.png";
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
                StrokeDashArray = this.dashArray,
                Margin = new System.Windows.Thickness(padding),
            };
            base.AttachEventHandler(rectangle);
            this._canvas.Children.Add(rectangle);
            base.generateShapeContent();
            Canvas.SetTop(_canvas, _start.Y);
            Canvas.SetLeft(_canvas, _start.X);
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

        public override void SetDashStroke(DoubleCollection dash)
        {
            this.dashArray = dash;
            if (rectangle != null)
            {
                rectangle.StrokeDashArray = dash;
            }
        }

        public override void SetStrokeColor(SolidColorBrush color)
        {
            this._colorStroke = color;
            if (rectangle != null)
            {
                rectangle.Stroke = color;
            }
        }

        public override void SetStrokeFill(SolidColorBrush fill)
        {
            this._colorFill = fill;
            if (rectangle != null)
            {
                rectangle.Fill = fill;
            }
        }

        public override void SetStrokeThickness(double thickness)
        {
            this.strokeThickness = thickness;
            if (rectangle != null)
            {
                rectangle.StrokeThickness = thickness;
            }
        }

    }
}
