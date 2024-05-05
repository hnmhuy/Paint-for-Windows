using BaseShapes;
using Paint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Commands
{
    public class PasteCommand : Command
    {
        private Paper receiver;
        public PasteCommand(Paper receiver, BaseShape prototype)
        {
            this.receiver = receiver;
            this.backup = prototype;
        }
        public override void Execute()
        {
            receiver.AddShape(backup);
        }

        public override void Undo()
        {
            this.receiver.RemoveShape(backup);
        }
    }
}
