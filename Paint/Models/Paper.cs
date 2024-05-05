using BaseShapes;
using Paint.Controller;
using System.IO;
using System.Windows.Controls;

namespace Paint.Models
{
    public class Paper
    {
        private Canvas _content;
        private List<BaseShape> drawnShapes = new List<BaseShape>();
        public List<BaseShape> DrawnShapes { get { return drawnShapes; } }
        public Canvas Content { get { return _content; } }

        public Paper(Canvas mainPage)
        {
            _content = mainPage;
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
                string name = binaryReader.ReadString();
                BaseShape shape = ShapeFactory.CreateShape(types, name, binaryReader);
                if (shape != null)
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

        public void ChangeToSelect(bool isSelect)
        {
            foreach (var shape in drawnShapes)
            {
                shape.CanSelect = isSelect;
            }
        }

        public void ChangeToAddText(bool isAddingText)
        {
            foreach (var shape in drawnShapes)
            {
                shape.CanAddText = isAddingText;
            }
        }


        public void Replace(BaseShape oldShape, BaseShape newShape)
        {
            int index = drawnShapes.IndexOf(oldShape);
            if (index != -1)
            {
                // Remove the old shape
                drawnShapes.Remove(oldShape);
                // Find the index of the old shape in the content
                int contentIndex = _content.Children.IndexOf(oldShape.content);
                // Remove the old shape from the content
                _content.Children.Remove(oldShape.content);
                // Add the new shape to the content
                _content.Children.Insert(contentIndex, newShape.content);
                // Add the new shape to the list
                drawnShapes.Insert(index, newShape);
            }
        }
    }
}
