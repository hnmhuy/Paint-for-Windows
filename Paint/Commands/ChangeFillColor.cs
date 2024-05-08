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
    public class ChangeFillColor : Command
    {
        private Paper reciever;
        private BaseShape newShape;
        private SolidColorBrush newColor;

        public ChangeFillColor(Paper reciever, BaseShape shape, SolidColorBrush newColor)
        {
            this.reciever = reciever;
            this.newColor = newColor;
            this.backup = shape;
        }

        public override void Execute()
        {
            if (newShape == null)
            {
                newShape = (BaseShape)this.backup.Clone();
                newShape.SetStrokeFill(newColor);
                newShape.Render();
            }
            Canvas.SetTop(newShape.content, newShape.Start.Y);
            Canvas.SetLeft(newShape.content, newShape.Start.X);
            reciever.Replace(this.backup, newShape);
            ShapeSelector.Instance.SelectShape(newShape);
        }

        public override void Undo()
        {
            reciever.Replace(newShape, this.backup);
            ShapeSelector.Instance.SelectShape(this.backup);
        }
    }
}
