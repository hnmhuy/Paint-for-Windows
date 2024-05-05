using BaseShapes;
using Paint.Models;
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
        public  BaseShape NewShape { get { return newShape; } }

        public ShapeMoveCommand(ShapeSelector selector, Point movingDistance, Paper receiver)
        {
            this.selector = selector;
            this.backup = selector.SelectedShape;
            this.movingDistance = movingDistance;
            this.receiver = receiver;
        }   

        public override void Execute()
        {
            newShape = (BaseShape)this.backup.Clone();
            newShape.Render();
            newShape.Move(movingDistance);         
            receiver.Replace(this.backup, newShape);   
            selector.SelectShape(newShape);
        }

        public override void Undo()
        {
            receiver.Replace(newShape, this.backup);
            selector.SelectShape(this.backup);
        }
    }
}
