using BaseShapes;
using Paint.Controller;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint.Models
{
    public class Paper : INotifyPropertyChanged
    {
        private static double previewWidth = 120;
        private static double previewHeight = 100;
        private Canvas _content;
        private List<BaseShape> drawnShapes = new List<BaseShape>();
        private List<BaseShape> previewShapes = new List<BaseShape>();  

        private Canvas _preivewLayer = new Canvas() { 
            AllowDrop = false, 
            IsEnabled = false,
            Width = previewWidth,
            Height = previewHeight,
            Background = Brushes.Wheat
        };
        public Canvas PreviewLayer { get { return _preivewLayer; } }

        public event PropertyChangedEventHandler? PropertyChanged;
        // Implement the OnPropertyChanged method
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public List<BaseShape> DrawnShapes { get { return drawnShapes; } }
        public Canvas Content { get { return _content; } }

        public Paper(Canvas mainPage)
        {
            _content = mainPage;
        }

        private void AddShapeReview(BaseShape shape, int index = -1)
        {
            BaseShape temp = (BaseShape)shape.Clone();
            if(index == -1)
            {
                previewShapes.Add(temp);
            } else
            {
                previewShapes.Insert(index, temp);
            }
            double scaleX = previewWidth / _content.ActualWidth;
            double scaleY = previewHeight / _content.ActualHeight;
            temp.SetPosition(new Point(temp.Start.X * scaleX, temp.Start.Y * scaleY), new Point(temp.End.X * scaleX, temp.End.Y * scaleY));
            temp.Render();

            if (index == -1)
            {
                _preivewLayer.Children.Add(temp.content);
            } else
            {
                _preivewLayer.Children.Insert(index, temp.content);
            }
            Canvas.SetTop(temp.content, temp.Start.Y);
            Canvas.SetLeft(temp.content, temp.Start.X);

            if (shape.ContentOnShape!= null)
            {
                // Find and remove the RichTextBox from the content
                foreach (UIElement element in temp.content.Children)
                {
                    if (element is RichTextBox)
                    {
                        temp.content.Children.Remove(element);
                        break;
                    }
                }
            }

            OnPropertyChanged(nameof(PreviewLayer));
        }

        public void AddShape(BaseShape shape)
        {
            drawnShapes.Add(shape);
            _content.Children.Add(shape.content);
            Canvas.SetTop(shape.content, shape.Start.Y);
            Canvas.SetLeft(shape.content, shape.Start.X);
            OnPropertyChanged(nameof(DrawnShapes));
            AddShapeReview(shape);
        }

        public void RemoveShape(BaseShape shape)
        {
            drawnShapes.Remove(shape);
            _content.Children.Remove(shape.content);
            int index = previewShapes.FindIndex(baseShape => baseShape.Id == shape.Id);
            _preivewLayer.Children.Remove(previewShapes[index].content);
            previewShapes.RemoveAt(index);
            OnPropertyChanged(nameof(DrawnShapes));
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

        public void Load(BinaryReader binaryReader, List<BaseShape> types, ShapeSelector selector)
        {
            int count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = binaryReader.ReadString();
                BaseShape shape = CreateShape(types, name, binaryReader);
                if (shape != null) 
                {
                    AddShape(shape);
                }
            }
        }

        public void Clear()
        {
            foreach(BaseShape shape in drawnShapes)
            {
                RemoveShape(shape);
            }
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
            // Finding old shape basing on id
            int index = drawnShapes.FindIndex(shape => shape.Id == oldShape.Id);

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

                _preivewLayer.Children.Remove(previewShapes[index].content);
                AddShapeReview(newShape, index);

            } else
            {
                Debug.WriteLine("Cannot find the shape to replace");    
            }
        }

        public void SetVisibility(bool isVisible)
        {
            Visibility visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            foreach (var shape in drawnShapes)
            {
                shape.content.Visibility = visibility;
            }
        }

        BaseShape CreateShape(List<BaseShape> prototypes, string name, BinaryReader reader)
        {
            BaseShape shape = null;
            foreach (BaseShape prototype in prototypes)
            {
                if (prototype.Name == name)
                {
                    shape = (BaseShape)prototype.Clone();
                    shape.Load(reader);
                    shape.Render();
                    break;
                }
            }
            return shape;
        }
    }
}
