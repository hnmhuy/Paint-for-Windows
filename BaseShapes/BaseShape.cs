using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BaseShapes
{
    public abstract class BaseShape : ICloneable
    {
        protected Canvas _canvas;
        protected String _name;
        protected String _iconName;
        protected Point _start;
        protected Point _end;
        protected SolidColorBrush _colorStroke;
        protected SolidColorBrush _colorFill;
        protected double strokeThickness;
        protected DoubleCollection dashArray;

        public BaseShape()
        {
            _start = new Point(0, 0);
            _end = new Point(0, 0);
            strokeThickness = 1;
            _colorStroke = new SolidColorBrush(Colors.Black);
            _colorFill = new SolidColorBrush(Colors.Transparent);
        }
        public String IconName { get { return _iconName; } }
        public String Name { get { return _name; } }
        public Point Start { get { return _start; } }
        public Point End { get { return _end; } }

        public void SetPosition(Point start, Point end)
        {
            // Ensure start position is always smaller than end position
            double minX = Math.Min(start.X, end.X);
            double minY = Math.Min(start.Y, end.Y);
            double maxX = Math.Max(start.X, end.X);
            double maxY = Math.Max(start.Y, end.Y);

            _start = new Point(minX, minY);
            _end = new Point(maxX, maxY);
        }

        public void SetProportionalPosition(Point start, Point end)
        {
            double width = Math.Abs(start.X - end.X);
            double height = Math.Abs(start.Y - end.Y);
            double size = Math.Min(width, height);
            if (end.X > start.X)
            {
                if (end.Y < start.Y)
                {
                    _start.X = start.X;
                    _start.Y = start.Y - size;
                    _end.X = start.X + size;
                    _end.Y = start.Y;
                }
                else
                {
                    _start = start;
                    _end.X = start.X + size;
                    _end.Y = start.Y + size;
                }
            }
            else
            {
                if (end.Y > start.Y)
                {
                    _start.X = start.X - size;
                    _start.Y = start.Y;
                    _end.X = start.X;
                    _end.Y = start.Y + size;
                }
                else
                {
                    _start.X = start.X - size;
                    _start.Y = start.Y - size;
                    _end = start;
                }
            }
        }

        public abstract object Clone();
        public abstract Canvas Render();
        public abstract void Resize();
    }
}
