using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BaseShapes
{
    public class ShapeSelector
    {
        private static double pointRadius = 10;
        private static Canvas border = new Canvas() { AllowDrop = false, Width = 0, Height = 0, Background = Brushes.Transparent, };
        public static Canvas Border { get { return border; } }
        private static Ellipse[] points = new Ellipse[9];
        private static ShapeSelector instance = new ShapeSelector();
        public static ShapeSelector Instance { get { return instance; } }

        private ShapeSelector() { 
            Rectangle rect = new Rectangle()
            {
                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 2 }),
                Fill = Brushes.Transparent,
            };
            border.Children.Add(rect);

            double width = border.Width;
            double height = border.Height;

            for (int i=0; i<3; i++)
            {
                for (int j=0; j<3;j++)
                {
                    Ellipse point = createPoint();
                    point.SetValue(Canvas.LeftProperty, i * width / 2);
                    point.SetValue(Canvas.TopProperty, j * height / 2);

                    // Set margin to make sure the point is put on the line of rectangle
                    point.Margin = new System.Windows.Thickness(-pointRadius / 2);

                    points[3*i+j] = point;
                    if(i==1 && j==1)
                        point.Visibility = System.Windows.Visibility.Hidden;
                    border.Children.Add(point);
                }
            }

        }

        private void updateBorder(BaseShape shape)
        {
            border.Width = shape.End.X - shape.Start.X + 2 * BaseShape.padding;
            border.Height = shape.End.Y - shape.Start.Y + 2 * BaseShape.padding;
            var rect = border.Children[0] as Rectangle;
            rect.Width = border.Width;
            rect.Height = border.Height;

            for (int i=0; i<3; i++)
            {
                for (int j=0; j<3;j++)
                {
                    Ellipse point = points[3*i+j];
                    point.SetValue(Canvas.LeftProperty, i * border.Width / 2);
                    point.SetValue(Canvas.TopProperty, j * border.Height / 2);
                }
            }
        }

        private Ellipse createPoint()
        {
            return new Ellipse()
            {
                Width = pointRadius,
                Height = pointRadius,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
            };
        }

        private BaseShape selectedShape;
        public BaseShape SelectedShape { get { return selectedShape; } }

        public void SelectShape(BaseShape shape)
        {
            instance.DeselectShape();
            if (shape.CanSelect && shape != selectedShape)
            {                
                updateBorder(shape);
                shape.content.Children.Add(border);
                instance.selectedShape = shape;
            }
        }

        public void DeselectShape()
        {
            if (instance.selectedShape != null)
            {
                instance.selectedShape.content.Children.Remove(border);
            }
            instance.selectedShape = null;
        }
    }
}
