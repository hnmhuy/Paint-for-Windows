using BaseShapes;

namespace Paint.Commands
{
    public class CreateShapeCommand : Command
    {
        private Paper receiver;

        public CreateShapeCommand(BaseShape prototype, Paper receiver)
        {
            this.backup = prototype;
            this.receiver = receiver;
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
