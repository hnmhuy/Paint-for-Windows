using BaseShapes;
using Paint.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace Paint
{
    public enum ToolType
    {
        Draw,
        CopyToClipboard,
        None
    }
    public class PaintApplication : INotifyPropertyChanged
    {
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
            papers.Add(currPage = new Paper());
            undoCommand = new UndoCommand(commandHistory);
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
                        prototypes.Add((BaseShape)Activator.CreateInstance(type)!);
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
    }
}
