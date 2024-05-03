using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint.Commands
{
    public class CopyToClipboardHandler
    {
        private static CopyToClipboardHandler instance = new CopyToClipboardHandler();
        public static CopyToClipboardHandler Instance { get { return instance; } }
        // Preview shape for selection area
        private Rectangle previewRect;
        private Point startPoint;
        private Point endPoint;

        private bool isSelecting = false;
        public bool IsSelecting { get { return isSelecting; } set { isSelecting = value; } }
        public Point StartPoint { get { return startPoint; }  set { startPoint = value; } }
        public Point EndPoint { get { return endPoint; } set { endPoint = value; } }

        private CopyToClipboardHandler() {
            startPoint = new Point(0, 0);
            endPoint = new Point(0, 0);
            previewRect = new Rectangle()
            {
                Width = 0,
                Height = 0,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[] { 4, 2 }),
            };
        }

        public void StartSelecting(Point point, Canvas parent)
        {
            if (isSelecting) return;
            isSelecting = true;
            startPoint = point;
            endPoint = point;
            previewRect.Width = 0;
            previewRect.Height = 0;
            previewRect.SetValue(Canvas.LeftProperty, startPoint.X);
            previewRect.SetValue(Canvas.TopProperty, startPoint.Y);
            parent.Children.Add(previewRect);
        }

        public void UpdateSelecting(Point point)
        {
            if (!isSelecting) return;

            endPoint = point;
            double x = startPoint.X < endPoint.X ? startPoint.X : endPoint.X;
            double y = startPoint.Y < endPoint.Y ? startPoint.Y : endPoint.Y;
            double width = startPoint.X < endPoint.X ? endPoint.X - startPoint.X : startPoint.X - endPoint.X;
            double height = startPoint.Y < endPoint.Y ? endPoint.Y - startPoint.Y : startPoint.Y - endPoint.Y;
            previewRect.Width = width;
            previewRect.Height = height;
            previewRect.SetValue(Canvas.LeftProperty, x);
            previewRect.SetValue(Canvas.TopProperty, y);
        }

        public static void VerifyPoint()
        {
            double minX = Math.Min(Instance.StartPoint.X, Instance.EndPoint.X);
            double minY = Math.Min(Instance.StartPoint.Y, Instance.EndPoint.Y);
            double maxX = Math.Max(Instance.StartPoint.X, Instance.EndPoint.X);
            double maxY = Math.Max(Instance.StartPoint.Y, Instance.EndPoint.Y);
            Instance.StartPoint = new Point(minX, minY);
            Instance.EndPoint = new Point(maxX, maxY);
        }

        public static void Copy(Canvas canvas)
        {
            canvas.Children.Remove(Instance.previewRect);
            if (instance.startPoint == instance.endPoint) return;
            // Set the background to white temporarily
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            // Set the background to white for renderTargetBitmap
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight));
            drawingContext.Close();
            renderTargetBitmap.Render(drawingVisual);
            renderTargetBitmap.Render(canvas);

            // Crop the image to the selected area
            CroppedBitmap croppedBitmap = new CroppedBitmap(renderTargetBitmap, new Int32Rect((int)Instance.StartPoint.X, (int)Instance.StartPoint.Y, (int)(Instance.EndPoint.X - Instance.StartPoint.X), (int)(Instance.EndPoint.Y - Instance.StartPoint.Y)));

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
            using (var fs = System.IO.File.OpenWrite("selected_area.png"))
            {
                encoder.Save(fs);
            }

            Clipboard.SetImage(croppedBitmap);
        }
    }
}
