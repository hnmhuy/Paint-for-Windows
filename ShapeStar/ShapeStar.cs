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
            _name = "Star";
            _iconName = "star.png";
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

            System.Windows.Point Point1 = new System.Windows.Point(width / 2, 0);
            System.Windows.Point Point2 = new System.Windows.Point(width, height * 0.4);
            System.Windows.Point Point3 = new System.Windows.Point(width * 0.825, height);
            System.Windows.Point Point4 = new System.Windows.Point(width * 0.175, height);
            System.Windows.Point Point5 = new System.Windows.Point(0, height * 0.4);
            System.Windows.Point Point1_1 = new System.Windows.Point(width * 0.35, height * 0.4);
            System.Windows.Point Point1_2 = new System.Windows.Point(width * 0.65, height * 0.4);
            System.Windows.Point Point1_3 = new System.Windows.Point((width * 127) / 176, (height * 13) / 22);
            System.Windows.Point Point1_4 = new System.Windows.Point(width / 2, (height * 26) / 35);
            System.Windows.Point Point1_5 = new System.Windows.Point((width * 49) / 176, (height * 13) / 22);
            PointCollection myPointCollection = new PointCollection();
            myPointCollection.Add(Point1);
            myPointCollection.Add(Point1_2);
            myPointCollection.Add(Point2);
            myPointCollection.Add(Point1_3);
            myPointCollection.Add(Point3);
            myPointCollection.Add(Point1_4);
            myPointCollection.Add(Point4);
            myPointCollection.Add(Point1_5);
            myPointCollection.Add(Point5);
            myPointCollection.Add(Point1_1);
            star.Points = myPointCollection;


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

                System.Windows.Point Point1 = new System.Windows.Point(width / 2, 0);
                System.Windows.Point Point2 = new System.Windows.Point(width, height * 0.4);
                System.Windows.Point Point3 = new System.Windows.Point(width * 0.825, height);
                System.Windows.Point Point4 = new System.Windows.Point(width * 0.175, height);
                System.Windows.Point Point5 = new System.Windows.Point(0, height * 0.4);
                System.Windows.Point Point1_1 = new System.Windows.Point(width * 0.35, height * 0.4);
                System.Windows.Point Point1_2 = new System.Windows.Point(width * 0.65, height * 0.4);
                System.Windows.Point Point1_3 = new System.Windows.Point((width * 127) / 176, (height * 13) / 22);
                System.Windows.Point Point1_4 = new System.Windows.Point(width / 2, (height * 26) / 35);
                System.Windows.Point Point1_5 = new System.Windows.Point((width * 49) / 176, (height * 13) / 22);
                PointCollection myPointCollection = new PointCollection();
                myPointCollection.Add(Point1);
                myPointCollection.Add(Point1_2);
                myPointCollection.Add(Point2);
                myPointCollection.Add(Point1_3);
                myPointCollection.Add(Point3);
                myPointCollection.Add(Point1_4);
                myPointCollection.Add(Point4);
                myPointCollection.Add(Point1_5);
                myPointCollection.Add(Point5);
                myPointCollection.Add(Point1_1);
                star.Points = myPointCollection;

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
            if(star  != null)
            {
                star.StrokeThickness = thickness;
            }
        }
    }

}
