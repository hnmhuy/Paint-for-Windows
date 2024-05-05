
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml.Permissions;

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
        protected bool isDashStroke = true;
        protected bool canSelect = false;
        protected bool canAddText = false;
        protected string contentOnShape;
        protected SolidColorBrush textColor;
        protected SolidColorBrush textBackgroundColor;
        protected ShapeSelector selector;
        public static double padding = 2;
        protected Cursor previousCursor;
        protected String id;
       
        public ShapeSelector Selector { get { return selector; } set { selector = value; } }
        public bool CanSelect { get { return canSelect; } set { canSelect = value; } }
        public bool CanAddText{ get { return canAddText; } set { canAddText = value; } }

        public static String GenerateId()
        {
            // Base on time
            return DateTime.Now.Ticks.ToString();
        }

        public string ContentOnShape { get { return contentOnShape; } set { contentOnShape = value; } }
        public SolidColorBrush TextColor { get { return textColor; } set { textColor = value; } }
        public SolidColorBrush TextBackgroundColor { get { return textBackgroundColor; } set { textBackgroundColor= value; } }
        public BaseShape()
        {
            id = GenerateId();
            _start = new Point(0, 0);
            _end = new Point(0, 0);
            strokeThickness = 1;
            _colorStroke = new SolidColorBrush(Colors.Black);
            _colorFill = new SolidColorBrush(Colors.Transparent);
        }

        public String IconName { get { return _iconName; } }
        public String Name { get { return _name; } }
        public Canvas content { get { return _canvas; } }
        public Point Start { get { return _start; } }
        public Point End { get { return _end; } }
        public String Id { get { return id; } }

        public virtual void SetPosition(Point start, Point end)
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

        public void generateShapeContent()
        {
            if (contentOnShape == null || _canvas == null)
                return;
            RichTextBox richTextBox = new RichTextBox()
            {
                Width = _end.X - _start.X - 10,
            };
            FlowDocument document = (FlowDocument)XamlReader.Parse(contentOnShape);
            // Set the FlowDocument as the content of the RichTextBox
            richTextBox.Document = document;
            richTextBox.IsEnabled = false;
            richTextBox.BorderBrush = Brushes.Transparent;
            richTextBox.Foreground = textColor;
            richTextBox.Background = textBackgroundColor;
            _canvas.Children.Add(richTextBox);
            Canvas.SetLeft(richTextBox, 5);
            double height = _end.Y - _start.Y;  
            Canvas.SetTop(richTextBox, (height - richTextBox.ActualHeight)/2);
        }

        public abstract object Clone();
        public abstract void Resize();
        public abstract void SetStrokeColor(SolidColorBrush color);
        public abstract void SetStrokeFill(SolidColorBrush fill);
        public abstract void SetStrokeThickness(double thickness);
        public abstract void SetDashStroke(DoubleCollection dash);
        public abstract Canvas Render();

        // Save the shape to a file
        public virtual void Save(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(_start.X);
            writer.Write(_start.Y);
            writer.Write(_end.X);
            writer.Write(_end.Y);
            writer.Write(_colorStroke.Color.ToString());
            writer.Write(_colorFill.Color.ToString());
            writer.Write(strokeThickness);
            isDashStroke = dashArray != null;
            writer.Write(isDashStroke);
            if (dashArray!=null)
            {
                writer.Write(dashArray.Count);
                foreach (double dash in dashArray)
                {
                    writer.Write(dash);
                }
            }
            if (contentOnShape != null)
            {
                writer.Write(true);
                writer.Write(contentOnShape);
                writer.Write(textColor.Color.ToString());
                writer.Write(textBackgroundColor.Color.ToString());

            }
            else
            {
                writer.Write(false);
            }
        }

        // Loading shapes from file
        public virtual void Load(BinaryReader reader)
        {
            id = reader.ReadString();
            _start = new Point(reader.ReadDouble(), reader.ReadDouble());
            _end = new Point(reader.ReadDouble(), reader.ReadDouble());
            _colorStroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(reader.ReadString()));
            _colorFill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(reader.ReadString()));
            strokeThickness = reader.ReadDouble();
            isDashStroke = reader.ReadBoolean();
            if (isDashStroke)
            {
                int count = reader.ReadInt32();
                dashArray = new DoubleCollection();
                for (int i = 0; i < count; i++)
                {
                    dashArray.Add(reader.ReadDouble());
                }
            }
            bool haveText = reader.ReadBoolean();
            if(haveText)
            {
                contentOnShape = reader.ReadString();
                textColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(reader.ReadString()));
                textBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(reader.ReadString()));
            }
        }

        public void AttachEventHandler(UIElement element)
        {           
            element.MouseEnter += (sender, e) =>
            {
                if (previousCursor == null || previousCursor != Mouse.OverrideCursor)
                {
                    previousCursor = Mouse.OverrideCursor;
                }
                if (canSelect)
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                } 
            };
            element.MouseLeave += (sender, e) =>
            {
                Mouse.OverrideCursor = previousCursor;
            };

            element.MouseDown += (sender, e) =>
            {
                selector.SelectShape(this);
                Debug.WriteLine("Change selected shape: " + this.Start.X + ";" + this.Start.Y);
            };
           
        }

        public void Move(Point movingDistance)
        {
            _start.X += movingDistance.X;
            _start.Y += movingDistance.Y;
            _end.X += movingDistance.X;
            _end.Y += movingDistance.Y;

            Canvas.SetTop(_canvas, _start.Y);
            Canvas.SetLeft(_canvas, _start.X);
        }
    }
}
