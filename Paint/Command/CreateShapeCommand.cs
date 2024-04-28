using BaseShapes;

namespace Paint.Command
{
    public class CreateShapeCommand : Command
    {
        private BaseShape prototype;
        private Paper receiver;

        public CreateShapeCommand(BaseShape prototype, Paper receiver)
        {
            this.prototype = prototype;
            this.receiver = receiver;
        }

        public override void Execute()
        {
            this.backup = (BaseShape)prototype.Clone();
            receiver.AddShape(prototype);
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
