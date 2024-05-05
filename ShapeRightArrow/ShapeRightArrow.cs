using BaseShapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ShapeRightArrow
{
    public class ShapeRightArrow : BaseShape
    {
        private Polygon rightArrow;
        public ShapeRightArrow()
        {
            _name = nameof(ShapeRightArrow);
            _iconName = "right_arrow.png";
            id = BaseShape.GenerateId();

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

            rightArrow = new Polygon();
            rightArrow.Fill = _colorFill;
            rightArrow.Stroke = _colorStroke;
            rightArrow.StrokeThickness = strokeThickness;
            rightArrow.StrokeDashArray = this.dashArray;
            rightArrow.Points = CalculateRightArrowPoints(width, height);

            base.AttachEventHandler(rightArrow);
            base.generateShapeContent();

            _canvas.Children.Add(rightArrow);

            return _canvas;
        }

        private PointCollection CalculateRightArrowPoints(double width, double height)
        {
            var points = new PointCollection()
            {
                new System.Windows.Point(width , height / 2),           // Point 1
                new System.Windows.Point(width / 2, 0),                 // Point 2
                new System.Windows.Point(width / 2, height / 4),        // Point 3
                new System.Windows.Point(0, height / 4),                // Point 4
                new System.Windows.Point(0, height * 0.75),             // Point 5
                new System.Windows.Point(width / 2, height * 0.75),     // Point 6
                new System.Windows.Point(width / 2, height),            // Point 7
            };

            return points;
        }

        public override void Resize()
        {
            if (rightArrow != null)
            {
                double width = Math.Abs(_end.X - _start.X);
                double height = Math.Abs(_end.Y - _start.Y);

                rightArrow.Points.Clear();
                rightArrow.Points = CalculateRightArrowPoints(width, height); 
            }
        }

        public override void SetDashStroke(DoubleCollection dash)
        {
            this.dashArray = dash;
            if (rightArrow != null)
            {
                rightArrow.StrokeDashArray = dash;
            }
        }

        public override void SetStrokeColor(SolidColorBrush color)
        {
            this._colorStroke = color;
            if (rightArrow != null)
            {
                rightArrow.Stroke = color;
            }
        }

        public override void SetStrokeFill(SolidColorBrush fill)
        {
            this._colorFill = fill;
            if (rightArrow != null)
            {
                rightArrow.Fill = fill;
            }
        }

        public override void SetStrokeThickness(double thickness)
        {
            this.strokeThickness = thickness;
            if (rightArrow  != null)
            {
                rightArrow.StrokeThickness = thickness;
            }
        }
    }

}
