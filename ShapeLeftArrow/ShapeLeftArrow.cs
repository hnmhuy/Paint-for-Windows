using BaseShapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
 
namespace ShapeLeftArrow
{
    public class ShapeLeftArrow: BaseShape
    {
        private Polygon leftArrow;
        public ShapeLeftArrow()
        {
            _name = "LeftArrow";
            _iconName = "left_arrow.png";
        }
        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override Canvas Render()
        {
            _canvas = new Canvas();

            double width = Math.Abs(_end.X - _start.X);
            double height = Math.Abs(_end.Y - _start.Y);

            leftArrow = new Polygon();
            leftArrow.Fill = _colorFill;
            leftArrow.Stroke = _colorStroke;
            leftArrow.StrokeThickness = strokeThickness;
            leftArrow.StrokeDashArray = this.dashArray;
            leftArrow.Points = CalculateRightArrowPoints(width, height);


            _canvas.Children.Add(leftArrow);

            return _canvas;
        }

        private PointCollection CalculateRightArrowPoints(double width, double height)
        {
            var points = new PointCollection()
            {
                 new System.Windows.Point(0 , height / 2),              // Point 1
                new System.Windows.Point(width / 2, 0),                 // Point 2
                new System.Windows.Point(width / 2, height / 4),        // Point 3
                new System.Windows.Point(width, height / 4),            // Point 4
                new System.Windows.Point(width, height * 0.75),         // Point 5
                new System.Windows.Point(width / 2, height * 0.75),     // Point 6
                new System.Windows.Point(width / 2, height),            // Point 7
            };

            return points;
        }

        public override void Resize()
        {
            if (leftArrow != null)
            {
                double width = Math.Abs(_end.X - _start.X);
                double height = Math.Abs(_end.Y - _start.Y);

                leftArrow.Points.Clear();
                leftArrow.Points = CalculateRightArrowPoints(width, height);
            }
        }

        public override void SetDashStroke(DoubleCollection dash)
        {
            this.dashArray = dash;
            if (leftArrow != null)
            {
                leftArrow.StrokeDashArray = dash;
            }
        }

        public override void SetStrokeColor(SolidColorBrush color)
        {
            this._colorStroke = color;
            if (leftArrow != null)
            {
                leftArrow.Stroke = color;
            }
        }

        public override void SetStrokeFill(SolidColorBrush fill)
        {
            this._colorFill = fill;
            if (leftArrow != null)
            {
                leftArrow.Fill = fill;
            }
        }

        public override void SetStrokeThickness(double thickness)
        {
            this.strokeThickness = thickness;
            if (leftArrow != null)
            {
                leftArrow.StrokeThickness = thickness;
            }
        }
    }

}
