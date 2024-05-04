using BaseShapes;
using System.IO;
using System.Windows.Controls;

namespace Paint
{
    public class Paper
    {
        private Canvas _content;
        private List<BaseShape> drawnShapes = new List<BaseShape>();
        public List<BaseShape> DrawnShapes { get { return drawnShapes; } }
        public Canvas Content { get { return _content; } }

        public Paper()
        {
            _content = new Canvas();
        }

        public void AddShape(BaseShape shape)
        {
            drawnShapes.Add(shape);
            _content.Children.Add(shape.content);
            Canvas.SetTop(shape.content, shape.Start.Y);
            Canvas.SetLeft(shape.content, shape.Start.X);
        }

        public void RemoveShape(BaseShape shape)
        {
            drawnShapes.Remove(shape);
            _content.Children.Remove(shape.content);
        }

        public void Save(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(drawnShapes.Count);
            foreach (var shape in drawnShapes)
            {
                binaryWriter.Write(shape.Name);
                shape.Save(binaryWriter);
            }
        }

        public void Load(BinaryReader binaryReader, List<Type> types)
        {
            int count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                String name = binaryReader.ReadString();
                BaseShape shape = ShapeFactory.CreateShape(types, name, binaryReader);
                if (shape!= null)
                {
                    shape.Render();
                    AddShape(shape);
                }
            }
        }

        public void Clear()
        {
            drawnShapes.Clear();
            _content.Children.Clear();
        }

        public void RenderAll()
        {
            foreach (var shape in drawnShapes)
            {
                var content = shape.Render();
                _content.Children.Add(content);
                Canvas.SetTop(content, shape.Start.Y);
                Canvas.SetLeft(content, shape.Start.X);
            }
        }
    }
}
