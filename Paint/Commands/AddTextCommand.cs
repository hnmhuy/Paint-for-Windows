using BaseShapes;
using Paint.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Paint.Commands
{
    public class AddTextCommand : Command
    {
        private Paper receiver;
        private BaseShape newShape;
        private ShapeSelector selector;
        private string contentOnShape;
        private SolidColorBrush textColor;
        private SolidColorBrush textBackgroundColor;

        public BaseShape NewShape { get { return newShape; } }
        public AddTextCommand(ShapeSelector selector, Paper receiver, string content, SolidColorBrush textColor, SolidColorBrush backgroundColor) { 

            this.selector = selector;
            this.receiver = receiver;
            this.backup = selector.SelectedShape;
            this.textColor = textColor;
            this.contentOnShape = content;
            this.textBackgroundColor = backgroundColor;

        }
        public override void Execute()
        {
            if (newShape == null)
            {
                newShape = (BaseShape)this.backup.Clone();
                newShape.ContentOnShape = contentOnShape;
                newShape.TextColor = textColor;
                newShape.TextBackgroundColor = textBackgroundColor;
                newShape.Render();
            }

            Canvas.SetTop(newShape.content, newShape.Start.Y);
            Canvas.SetLeft(newShape.content, newShape.Start.X);
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
