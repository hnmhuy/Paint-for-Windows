using BaseShapes;
using Microsoft.Win32;
using Paint.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint
{
    public enum ToolType
    {
        Draw,
        CopyToClipboard,
        Select,
        MovingShape,
        None
    }
    public class PaintApplication : INotifyPropertyChanged
    {
        private Grid drawSpace;
        public Grid DrawSpace
        {
            get { return drawSpace; }
            set
            {
                drawSpace = value;
                OnPropertyChanged(nameof(DrawSpace));
            }
        }
        private ToolType currentTool = ToolType.None;
        public ToolType CurrentTool
        {
            get { return currentTool; }
            set
            {
                currentTool = value;
                OnPropertyChanged(nameof(CurrentTool));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        // Implement INotifyPropertyChanged interface
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Atrributes
        private List<BaseShape> prototypes = new List<BaseShape>();
        private List<Paper> papers = new List<Paper>();
        private MouseEditor editor = new MouseEditor();
        private bool isDrawing = false;
        private StackPanel shapeStack;
        private BaseShape currPrototype;
        private Point initalPoint;
        private Paper currPage;
        private Canvas drawingShape;
        public Paper CurrentPage { get { return currPage; } }
        public List<Type> loadedType = new List<Type>();
        private Canvas mainPage = new Canvas();
        public Canvas MainPage { get { return mainPage; } }
        public ShapeSelector selector = ShapeSelector.Instance; 



        // Brush attribute
        private double thickness = 1;
        public double Thickness
        {
            get { return thickness; }
            set
            {
                thickness = value;
                OnPropertyChanged(nameof(Thickness));
                StrokeThicknessChanged();
            }
        }

        private DoubleCollection strokeType = new DoubleCollection();
        public DoubleCollection StrokeType
        {
            get { return  strokeType; } 
            set 
            {
                strokeType = value;
                OnPropertyChanged(nameof(StrokeType));
                DashStrokeChanged();

            }
        }

        private SolidColorBrush strokeColor = Brushes.Black;
        public SolidColorBrush StrokeColor
        {
            get { return strokeColor; }
            set
            {
                strokeColor = value;
                OnPropertyChanged(nameof(StrokeColor));
                ColorStrokeChanged();
            }
        }
        private SolidColorBrush fillColor = Brushes.Transparent;
        public SolidColorBrush FillColor
        {
            get { return fillColor; }
            set
            {
                fillColor = value;
                OnPropertyChanged(nameof(FillColor));
                ColorFillChanged();
            }
        }

        // Control attribute
        private CommandHistory commandHistory = new CommandHistory();
        private UndoCommand undoCommand;
        private bool canUndo = false;
        public bool CanUndo
        {
            get { return canUndo; }
            set
            {
                canUndo = value;
                OnPropertyChanged(nameof(CanUndo));
            }
        }
        private bool canRedo = false;
        public bool CanRedo
        {
            get { return canRedo; }
            set
            {
                canRedo = value;
                OnPropertyChanged(nameof(CanRedo));
            }
        }

        // Copy to clipboard handler
        public CopyToClipboardHandler copyToClipboardHandler = CopyToClipboardHandler.Instance;

        // Constructor
        public PaintApplication()
        {
            LoadPrototypes();
            papers.Add(currPage = new Paper(mainPage));
            undoCommand = new UndoCommand(commandHistory);
            SelectorMouseHandler();
        }
        private void Canvas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Identify the clicked canvas
            Canvas clickedCanvas = sender as Canvas;
            if (clickedCanvas != null)
            {
                // Perform actions based on the clicked canvas
                Debug.WriteLine("Canvas double-clicked: " + clickedCanvas.Name);

                // For example, you can get the shape associated with this canvas and change its stroke color
                ChangeStrokeColor(clickedCanvas, Brushes.Red);
            }
        }
        private void ChangeStrokeColor(Canvas canvas, SolidColorBrush newStrokeColor)
        {
            foreach (UIElement element in canvas.Children)
            {
                if (element is Shape shape)
                {
                    // Check if the shape has a Stroke property (e.g., Rectangle, Ellipse, etc.)
                    if (shape.Stroke != null)
                    {
                        shape.Stroke = newStrokeColor;
                    }
                }
            }
        }

        private void LoadPrototypes()
        {
            String folder = AppDomain.CurrentDomain.BaseDirectory;
            var files = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && (typeof(BaseShape).IsAssignableFrom(type)) && !type.IsAbstract)
                    {
                        Debug.WriteLine("Got type " + type.Name);
                        BaseShape shape = (BaseShape)Activator.CreateInstance(type)!;
                        shape.Selector = selector;
                        prototypes.Add(shape);
                        loadedType.Add(type);
                    }
                }
            }
        }
        public void GenerateShapeControls(StackPanel stack)
        {
            shapeStack = stack;
            String folder = AppDomain.CurrentDomain.BaseDirectory + "//Assets//Icon";
            foreach (var item in prototypes)
            {
                var uriSource = new Uri(folder + "//" + item.IconName);
                var iconImage = new System.Windows.Controls.Image();
                iconImage.Source = new BitmapImage(uriSource);
                var button = new ToggleButton()
                {
                    Height = 39,
                    Tag = item,
                    FontSize = 12,
                    Margin = new System.Windows.Thickness(8)
                };
                StackPanel stackPnl = new StackPanel();
                stackPnl.Orientation = Orientation.Horizontal;
                stackPnl.Children.Add(iconImage);

                button.Content = stackPnl;
                button.Click += ShapeControl_Click;
                stack.Children.Add(button);

            }
        }

        public void UnselectShape()
        {
            foreach (var item in shapeStack.Children)
            {
                ((ToggleButton)item).IsChecked = false;
            }
        }

        public void ShapeControl_Click(object sender, RoutedEventArgs e)
        {
            UnselectShape();
            ((ToggleButton)sender).IsChecked = true;
            currPrototype = (BaseShape)((ToggleButton)sender).Tag;
            CurrentTool = ToolType.Draw;
        }
        public void StartDrawing(Point point)
        {
            if (currPrototype == null) return;
            initalPoint = point;
            isDrawing = true;
            currPrototype.SetPosition(point, point);
            drawingShape = currPrototype.Render();
            currPage.Content.Children.Add(drawingShape);
        }
        public void Drawing(Point end, Boolean isShift = false)
        {
            if (isDrawing)
            {
                if (isShift)
                {
                    currPrototype.SetProportionalPosition(initalPoint, end);
                }
                else currPrototype.SetPosition(initalPoint, end);
                currPrototype.Resize();
                Canvas.SetTop(drawingShape, currPrototype.Start.Y);
                Canvas.SetLeft(drawingShape, currPrototype.Start.X);
                
            }
        }
        public void DrawComplete()
        {
            if (!isDrawing || currPrototype == null) return;
            isDrawing = false;
            if (initalPoint == currPrototype.End) return;
            currPage.Content.Children.Remove(drawingShape);
            BaseShape generatedShape = (BaseShape)currPrototype.Clone();
            generatedShape.Render();
            CreateShapeCommand command = new CreateShapeCommand(generatedShape, currPage);
            this.ExecuteCommand(command);
        }

        private void ExecuteCommand(Command command)
        {
            editor.SetCommand(command);
            editor.ExecuteCommand();
            commandHistory.AddCoomand(command);
            UpdateHistoryState();
        }

        private void UpdateHistoryState()
        {
            CanUndo = commandHistory.CanUndo();
            CanRedo = commandHistory.CanRedo();
        }

        private void StrokeThicknessChanged()
        {
            foreach (var item in prototypes)
            {
                item.SetStrokeThickness(thickness);
            }
        }

        private void DashStrokeChanged()
        {
            foreach (var item in prototypes)
            {
               
                item.SetDashStroke(strokeType);
            }
        }

        private void ColorStrokeChanged()
        {
            foreach (var item in prototypes)
            {
                item.SetStrokeColor(strokeColor);
            }
        }

        private void ColorFillChanged()
        {
            foreach (var item in prototypes)
            {
                item.SetStrokeFill(fillColor);
            }
        }

        public void Undo()
        {
            undoCommand.Execute();
            UpdateHistoryState();
        }

        public void Redo()
        {
            undoCommand.Undo();
            UpdateHistoryState();
        }

        // SaveFile file
        public void SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();   
            // Filter for this application's file type (*.paint)
            saveFileDialog.Filter = "Paint files (*.paint)|*.paint";

            if (saveFileDialog.ShowDialog() == true)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.Create)))
                {
                    writer.Write(papers.Count);
                    foreach (var paper in papers)
                    {
                        paper.Save(writer);
                    }
                }
            }
        }

        // OpenFile file
        public void OpenFile()
        {
            papers.Clear();
            drawSpace.Children.Clear();
            mainPage.Children.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Filter for this application's file type (*.paint)
            openFileDialog.Filter = "Paint files (*.paint)|*.paint";

            if (openFileDialog.ShowDialog() == true)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog.FileName, FileMode.Open)))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        Paper paper = new Paper(mainPage);
                        paper.Load(reader, loadedType);
                        papers.Add(paper);
                        drawSpace.Children.Add(paper.Content);
                    }
                }
            }

            currPage = papers[papers.Count - 1];
        }

        public void NewFile()
        {
            papers.Clear();
            mainPage.Children.Clear();
            drawSpace.Children.Clear();
            papers.Add(currPage = new Paper(mainPage));
            drawSpace.Children.Add(mainPage);
        }

        public bool IsEmpty()
        {
            bool isEmpty = true;
            foreach (var paper in papers)
            {
                if (paper.Content.Children.Count > 0)
                {
                    isEmpty = false;
                    break;
                }
            }
            return isEmpty;
        }

        public void ChangeToSelectingMode(bool isSelecting)
        {
            CurrentTool = isSelecting ? ToolType.Select : ToolType.None;
            foreach(var page in papers)
            {
                page.ChangeToSelect(isSelecting);
            }
        }

        public void SelectorMouseHandler()
        {
            Canvas bounder = ShapeSelector.Border;
            Rectangle? rect = bounder.Children[0] as Rectangle;
            Cursor currCursor = Mouse.OverrideCursor;
            
            if (rect!=null)
            {
                rect.MouseEnter += (sender, e) =>
                {
                    if (currCursor != Mouse.OverrideCursor)
                    {
                        currCursor = Mouse.OverrideCursor;
                    }
                    Mouse.OverrideCursor = Cursors.SizeAll;
                };

                rect.MouseLeave += (sender, e) =>
                {
                    Mouse.OverrideCursor = currCursor;
                };

                rect.MouseDown += (sender, e) =>
                {
                    selector.SelectedShape.content.Opacity = 0.8;
                    initalPoint = e.GetPosition(mainPage);
                    currentTool = ToolType.MovingShape;
                };
            }
        }

        public void OnMovingShape(Point end)
        {
            double deltaX = end.X - initalPoint.X;
            double deltaY = end.Y - initalPoint.Y;
            selector.SelectedShape.content.SetValue(Canvas.LeftProperty, selector.SelectedShape.Start.X + deltaX);
            selector.SelectedShape.content.SetValue(Canvas.TopProperty, selector.SelectedShape.Start.Y + deltaY);
        }

        public void OnMovingShapeComplete(Point newPoint)
        {
            selector.SelectedShape.content.Opacity = 1;            
            currentTool = ToolType.Select;
            double deltaX = newPoint.X - initalPoint.X;
            double deltaY = newPoint.Y - initalPoint.Y;
            ShapeMoveCommand command = new ShapeMoveCommand(selector.SelectedShape, new Point(deltaX, deltaY), currPage, selector);
            ExecuteCommand(command);
        }
    }
}
