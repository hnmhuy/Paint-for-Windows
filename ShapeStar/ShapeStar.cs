using BaseShapes;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace ShapeStar
{
    public class ShapeStar : BaseShape
    {
        private Polygon star;
        public ShapeStar()
        {
            _name = nameof(ShapeStar);
            _iconName = "star.png";
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

            star = new Polygon();
            star.Fill = _colorFill;
            star.Stroke = _colorStroke;
            star.StrokeThickness = strokeThickness;
            star.StrokeDashArray = this.dashArray;
            star.Points = CalculateStarPoints(width, height);
            _canvas.Children.Add(star);

            return _canvas;
        }

        public override void Resize()
        {
            if (star != null)
            {
                double width = Math.Abs(_end.X - _start.X);
                double height = Math.Abs(_end.Y - _start.Y);

                star.Points.Clear(); // Clear the points of the old star
                star.Points = CalculateStarPoints(width, height);

            }
        }

        private PointCollection CalculateStarPoints(double width, double height)
        {
            var points = new PointCollection()
            {
                new System.Windows.Point(width / 2, 0),                             // Point 1
                new System.Windows.Point(width * 0.65, height * 0.4),               // Point 1_2
                new System.Windows.Point(width, height * 0.4),                      // Point 2 
                new System.Windows.Point((width * 127) / 176, (height * 13) / 22),  // Point 1_3
                new System.Windows.Point(width * 0.825, height),                    // Point 3
                new System.Windows.Point(width / 2, (height * 26) / 35),            // Point 1_4
                new System.Windows.Point(width * 0.175, height),                    // Point 4
                new System.Windows.Point((width * 49) / 176, (height * 13) / 22),   // Point 1_5
                new System.Windows.Point(0, height * 0.4),                          // Point 5
                new System.Windows.Point(width * 0.35, height * 0.4),               // Point 1_1
            };
          
            return points;
        }


        public override void SetDashStroke(DoubleCollection dash)
        {
            this.dashArray = dash;
            if (star != null)
            {
                star.StrokeDashArray = dash;
            }
        }

        public override void SetStrokeColor(SolidColorBrush color)
        {
            this._colorStroke = color;
            if (star != null)
            {
                star.Stroke = color;
            }
        }

        public override void SetStrokeFill(SolidColorBrush fill)
        {
            this._colorFill = fill;
            if (star != null)
            {
                star.Fill = fill;
            }
        }

        public override void SetStrokeThickness(double thickness)
        {
            this.strokeThickness = thickness;
            if(star  != null)
            {
                star.StrokeThickness = thickness;
            }
        }
    }

}
