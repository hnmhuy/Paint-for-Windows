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
    public class ChangeStrokeType : Command
    {
        private Paper reciever;
        private BaseShape newShape;
        private DoubleCollection newStroke; 

        public ChangeStrokeType(Paper reciever, BaseShape shape, DoubleCollection newStroke)
        {
            this.reciever = reciever;
            this.newStroke = newStroke;
            this.backup = shape;
        }

        public override void Execute()
        {
            newShape = (BaseShape) this.backup.Clone();
            newShape.SetDashStroke(newStroke);
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
