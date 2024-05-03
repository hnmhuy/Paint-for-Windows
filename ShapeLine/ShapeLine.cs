using BaseShapes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeLine
{
    public class ShapeLine : BaseShape
    {
        private Line line;
        private Point startPoint;
        private Point endPoint;

        public ShapeLine() { 
            _name= "Line";
            _iconName = "line.png";
        }
        public override object Clone()
        {
            return MemberwiseClone();
        }
        public override void SetPosition(Point start, Point end)
        {
            //_start = new Point(start.X, start.Y);
            //_end = new Point(end.X, end.Y);
            double minX = Math.Min(start.X, end.X);
            double minY = Math.Min(start.Y, end.Y);
            double maxX = Math.Max(start.X, end.X);
            double maxY = Math.Max(start.Y, end.Y);

            _start = new Point(minX, minY);
            _end = new Point(maxX, maxY);

            if (end.Y < start.Y && end.X > start.X)
            {
                startPoint = new Point(0, _end.Y - _start.Y);
                endPoint = new Point(_end.X - _start.X, 0);
            }
            else if (end.Y > start.Y && end.X > start.X)
            {
                startPoint = new Point(0, 0);
                endPoint = new Point(_end.X - _start.X, _end.Y - _start.Y);
            }
            else if (end.Y > start.Y && end.X < start.X)
            {
                startPoint = new Point(_end.X - _start.X, 0);
                endPoint = new Point(0, _end.Y - _start.Y);
            }
            else if (end.Y < start.Y && end.X < start.X)
            {
                startPoint = new Point(_end.X - _start.X, _end.Y - _start.Y);
                endPoint = new Point(0, 0);
            }
            else
            {
                startPoint = new Point(0, 0);
                endPoint = new Point(0, 0);
            }


        }
        public override Canvas Render()
        {
            _canvas = new Canvas();
            line = new Line()
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                StrokeDashArray = this.dashArray,
                StrokeThickness = strokeThickness,
                Stroke = this._colorStroke,
                Fill = this._colorFill,
            };

            this._canvas.Children.Add(line);
            return _canvas;
        }

        public override void Resize()
        {
            if(line != null)
            {
                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = endPoint.X;
                line.Y2 = endPoint.Y;
            }
  
        }

        public override void SetDashStroke(DoubleCollection dash)
        {
            this.dashArray = dash;
            if (line != null)
            {
                line.StrokeDashArray = dash;
            }
        }

        public override void SetStrokeColor(SolidColorBrush color)
        {
            this._colorStroke = color;
            if (line != null)
            {
                line.Stroke = color;
            }
        }

        public override void SetStrokeFill(SolidColorBrush fill)
        {
            this._colorFill = fill;
            if (line != null)
            {
                line.Fill = fill;
            }
        }

        public override void SetStrokeThickness(double thickness)
        {
            this.strokeThickness = thickness;
            if(line != null)
            {
                line.StrokeThickness = thickness;
            }
        }
    }

}
