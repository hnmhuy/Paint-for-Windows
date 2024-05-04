using BaseShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paint.Commands
{
    public class ShapeMoveCommand : Command
    {

        private Point movingDistance;
        private Paper receiver;
        private BaseShape newShape;
        private ShapeSelector selector; 

        public ShapeMoveCommand(BaseShape shape, Point movingDistance, Paper receiver, ShapeSelector selector)
        {
            this.backup = shape;
            this.movingDistance = movingDistance;
            this.receiver = receiver;
            this.selector = selector;
        }   

        public override void Execute()
        {
            newShape = (BaseShape)this.backup.Clone();
            newShape.Move(movingDistance);
            receiver.Replace(this.backup, newShape);
            selector.SelectedShape = newShape;
        }

        public override void Undo()
        {
            receiver.Replace(this.newShape, this.backup);
            Canvas content = newShape.content;
            Canvas.SetTop(content, this.backup.Start.Y);
            Canvas.SetLeft(content, this.backup.Start.X);

            selector.SelectedShape = this.backup;
        }
    }
}
