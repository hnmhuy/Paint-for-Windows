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
    public class ChangeStrokeWeight : Command
    {
        Paper reciever;
        private BaseShape newShape;
        private double newValue;

        public ChangeStrokeWeight(Paper reciever, BaseShape shape, double newStroke)
        {
            this.reciever = reciever;
            this.newValue = newStroke;
            this.backup = shape;
        }

        public override void Execute()
        {
            newShape = (BaseShape)this.backup.Clone();
            newShape.SetStrokeThickness(newValue);
            newShape.Render();
            Canvas.SetTop(newShape.content, newShape.Start.Y);
            Canvas.SetLeft(newShape.content, newShape.Start.X);
            reciever.Replace(this.backup, newShape);
            ShapeSelector.Instance.SelectShape(newShape);
        }

        public override void Undo()
        {
            reciever.Replace(newShape, this.backup);
            ShapeSelector.Instance.SelectShape(this.backup);
            ShapeSelector.Instance.SelectShape(this.Backup);
        }
    }
}
