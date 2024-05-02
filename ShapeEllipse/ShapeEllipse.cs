using BaseShapes;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeEllipse
{
    public class ShapeEllipse : BaseShape
    {
        private Ellipse ellipse;
        public ShapeEllipse() {
            _name = "Ellipse";
            _iconName = "ellipse.png";

        }
        public override object Clone()
        {
           return MemberwiseClone();
        }

        public override Canvas Render()
        {
            _canvas = new Canvas();
            ellipse = new Ellipse()
            {
                Width = Math.Abs(_start.X - _end.X),
                Height = Math.Abs(_start.Y - _end.Y),
                StrokeThickness = strokeThickness,
                Fill = this._colorFill,
                Stroke = this._colorStroke,
            };
            _canvas.Children.Add(ellipse);
            return _canvas;
        }

        public override void Resize()
        {
            if (ellipse != null)
            {
                ellipse.Width = Math.Abs(_start.X - _end.X);
                ellipse.Height = Math.Abs(_start.Y - _end.Y);
            }
        }

        public override void SetDashStroke(DoubleCollection dash)
        {
            throw new NotImplementedException();
        }

        public override void SetStrokeColor(SolidColorBrush color)
        {
            throw new NotImplementedException();
        }

        public override void SetStrokeFill(SolidColorBrush fill)
        {
            throw new NotImplementedException();
        }

        public override void SetStrokeThickness(double thickness)
        {
            this.strokeThickness = thickness;
            if (ellipse != null)
            {
                ellipse.StrokeThickness = thickness;
            }
        }
    }

}