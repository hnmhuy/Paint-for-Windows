using BaseShapes;
using Paint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Commands
{
    public class CutCommand : Command
    {
        private Paper receiver;
        public CutCommand(BaseShape prototype, Paper receiver) { 
            this.backup = prototype;
            this.receiver = receiver;
        }
        public override void Execute()
        {
            this.receiver.RemoveShape(backup);
        }

        public override void Undo()
        {
            receiver.AddShape(backup);
        }
    }
}
