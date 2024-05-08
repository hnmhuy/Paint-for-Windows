using BaseShapes;
using Paint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint.Commands
{
    public class StrokeColorCommand : Command
    {
        private Paper receiver;
        private BaseShape newShape;
        private SolidColorBrush newColor;

        public StrokeColorCommand(Paper receiver, BaseShape shape, SolidColorBrush newColor)
        {
            this.receiver = receiver;
            this.backup = shape;
            this.newColor = newColor;
        }

        public override void Execute()
        {
            if (newShape == null)
            {
                newShape = (BaseShape)this.backup.Clone();
                newShape.SetStrokeColor(newColor);
                newShape.Render();
            }
            Canvas.SetTop(newShape.content, newShape.Start.Y);
            Canvas.SetLeft(newShape.content, newShape.Start.X);
            receiver.Replace(this.backup, newShape);
            ShapeSelector.Instance.SelectShape(newShape);
        }

        public override void Undo()
        {
            receiver.Replace(newShape, this.backup);
            ShapeSelector.Instance.SelectShape(this.backup);
        }
    }
}
