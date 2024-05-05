using BaseShapes;
using Microsoft.Win32;
using Paint.Commands;
using Paint.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        AddText,
        None
    }
    public class PaintApplication : INotifyPropertyChanged
    {
        // ==== Attributes ====
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
        public List<BaseShape> prototypes = new List<BaseShape>();
        public ObservableCollection<Paper> Layers = new ObservableCollection<Paper>();
        private Paper currPage;
        public List<Type> loadedType = new List<Type>();
        public Paper CurrentPage { get { return currPage; } }
        private Canvas mainPage = new Canvas();
        public Canvas MainPage { get { return mainPage; } } 

        // ==== UI Elements ====
        private StackPanel shapeStack;
        private BaseShape currPrototype;
        private Canvas drawingShape;
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
        private StackPanel layerReview;
        public StackPanel LayerReview { 
            get { return layerReview; } 
            set { 
                layerReview = value;
                UpdateLayerReview();
            } 
        }

        // ==== Drawing attributes ====
        private double thickness = 1;
        private DoubleCollection strokeType = new DoubleCollection();
        private SolidColorBrush strokeColor = Brushes.Black;
        private SolidColorBrush fillColor = Brushes.Transparent;
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
        public DoubleCollection StrokeType
        {
            get { return strokeType; }
            set
            {
                strokeType = value;
                OnPropertyChanged(nameof(StrokeType));
                DashStrokeChanged();

            }
        }
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
        // ==== Interactive attributes ====
        private MouseEditor editor = new MouseEditor();
        public CopyToClipboardHandler copyToClipboardHandler = CopyToClipboardHandler.Instance;
        private CommandHistory commandHistory = new CommandHistory();
        public ShapeSelector selector = ShapeSelector.Instance;
        private UndoCommand undoCommand;
        private bool isDrawing = false;
        private bool canUndo = false;
        private bool canRedo = false;
        private Point initalPoint;
        public bool CanUndo
        {
            get { return canUndo; }
            set
            {
                canUndo = value;
                OnPropertyChanged(nameof(CanUndo));
            }
        }
        public bool CanRedo
        {
            get { return canRedo; }
            set
            {
                canRedo = value;
                OnPropertyChanged(nameof(CanRedo));
            }
        }

        // Implement INotifyPropertyChanged interface
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        

        // Constructor
        public PaintApplication()
        {
            LoadPrototypes();
            Layers.Add(currPage = new Paper(mainPage));
            undoCommand = new UndoCommand(commandHistory);
            SelectorMouseHandler(); 
        }

        //private void Layers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    UpdateLayerReview();
        //}

        // === Set up methods ===
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
        public void ShapeControl_Click(object sender, RoutedEventArgs e)
        {
            UnselectShape();
            ((ToggleButton)sender).IsChecked = true;
            currPrototype = (BaseShape)((ToggleButton)sender).Tag;
            ChangeToSelectingMode(false);
            selector.DeselectShape();
            CurrentTool = ToolType.Draw;
        }
        public void UnselectShape()
        {
            foreach (var item in shapeStack.Children)
            {
                ((ToggleButton)item).IsChecked = false;
            }
        }
        public void ChangeToSelectingMode(bool isSelecting)
        {
            CurrentTool = isSelecting ? ToolType.Select : ToolType.None;
            foreach (var page in Layers)
            {
                page.ChangeToSelect(isSelecting);
            }
        }
        public void ChangeToAddTextMode(bool isAddingText)
        {
            CurrentTool = isAddingText ? ToolType.AddText : ToolType.None;
            foreach (var page in Layers)
            {
                page.ChangeToAddText(isAddingText);
            }

        }

        private Grid GeneratePreviewLayer(Paper paper)
        {
            Grid stack = new Grid()
            {
                Margin = new Thickness(5),
            };
            stack.Children.Add(paper.PreviewLayer);

            ToggleButton button = new ToggleButton()
            {
                Width = 40,
                Height = 40,
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "//Assets//Icon//invisible.png")),
                    Stretch = System.Windows.Media.Stretch.Uniform
                },
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
               
            };

            button.Click += (sender, e) =>
            {
                if (button.IsChecked == true)
                {
                    Debug.WriteLine("Hide layer");
                    paper.SetVisibility(false);
                }
                else
                {
                    Debug.WriteLine("Show layer");
                    paper.SetVisibility(true);
                }
            };

            stack.MouseDown += (sender, e) =>
            {
                // Only for left mouse button
                if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
                {
                    currPage = paper;
                    Debug.WriteLine("Change to layer: " + Layers.IndexOf(paper));
                }
            };
            stack.Children.Add(button);
            return stack;
        }

        private void UpdateLayerReview()
        {
            foreach (var paper in Layers)
            {
                layerReview.Children.Add(GeneratePreviewLayer(paper));
            }
        }

        public void AddLayer()
        {
            Layers.Add(currPage = new Paper(mainPage));
            layerReview.Children.Add(GeneratePreviewLayer(currPage));
        }

        // ==== Mouse events ====

        //= Drawing
        private void Canvas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Identify the clicked canvas
            Canvas? clickedCanvas = sender as Canvas;
            if (clickedCanvas != null)
            {
                // Perform actions based on the clicked canvas
                Debug.WriteLine("Canvas double-clicked: " + clickedCanvas.Name);

                // For example, you can get the shape associated with this canvas and change its stroke color
                ChangeStrokeColor(clickedCanvas, Brushes.Red);
            }
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

        //= Moving shape
        public void SelectorMouseHandler()
        {
            Canvas bounder = ShapeSelector.Border;
            Rectangle? rect = bounder.Children[0] as Rectangle;
            Cursor currCursor = Mouse.OverrideCursor;

            if (rect != null)
            {
                rect.MouseEnter += (sender, e) =>
                {
                    if (currentTool != ToolType.AddText)
                    {
                        if (currCursor != Mouse.OverrideCursor)
                        {
                            currCursor = Mouse.OverrideCursor;
                        }
                        Mouse.OverrideCursor = Cursors.SizeAll;
                    }
                    else
                    {
                        Mouse.OverrideCursor = Cursors.IBeam;
                    }

                };
                rect.MouseLeave += (sender, e) =>
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                };

                rect.MouseDown += (sender, e) =>
                {
                    if (currentTool != ToolType.AddText)
                    {
                        selector.SelectedShape.content.Opacity = 0.8;
                        initalPoint = e.GetPosition(mainPage);
                        currentTool = ToolType.MovingShape;
                    }
                    else
                    {
                        currentTool = ToolType.AddText;
                        onAddingText();
                    }
                };
                rect.MouseMove += (sender, e) =>
                {
                    if (currentTool == ToolType.AddText)
                    {
                        Mouse.OverrideCursor = Cursors.IBeam;
                    }

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
            Mouse.OverrideCursor = Cursors.Arrow;
            currentTool = ToolType.Select;
            double deltaX = newPoint.X - initalPoint.X;
            double deltaY = newPoint.Y - initalPoint.Y;
            ShapeMoveCommand command = new ShapeMoveCommand(selector, new Point(deltaX, deltaY), currPage);
            ExecuteCommand(command);
            Debug.WriteLine("Move shape: " + selector.SelectedShape.Id);    
        }

        //= Add text
        public void onAddingText()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = "hahah";
            selector.SelectedShape.content.Children.Add(textBlock);
        }

        // Helper method to change prototyes' properties
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

        //==== Commands ====
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

        //= Files commands
        public void SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();   
            // Filter for this application's file type (*.paint)
            saveFileDialog.Filter = "Paint files (*.paint)|*.paint";

            if (saveFileDialog.ShowDialog() == true)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.Create)))
                {
                    writer.Write(Layers.Count);
                    foreach (var paper in Layers)
                    {
                        paper.Save(writer);
                    }
                }
            }
        }
        public void OpenFile()
        {
            Layers.Clear();
            LayerReview.Children.Clear();
            mainPage.Children.Clear();
            commandHistory.Clear();
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
                        paper.Load(reader, prototypes, selector);
                        Layers.Add(paper);
                    }
                }
            }
            currPage = Layers[Layers.Count - 1];
            UpdateLayerReview();
        }
        public void NewFile()
        {
            Layers.Clear();
            layerReview.Children.Clear();
            mainPage.Children.Clear();
            drawSpace.Children.Clear();
            Layers.Add(currPage = new Paper(mainPage));
            drawSpace.Children.Add(mainPage);
            UpdateLayerReview();
        }
        public bool IsEmpty()
        {
            bool isEmpty = true;
            foreach (var paper in Layers)
            {
                if (paper.Content.Children.Count > 0)
                {
                    isEmpty = false;
                    break;
                }
            }
            return isEmpty;
        }  
    }
}
