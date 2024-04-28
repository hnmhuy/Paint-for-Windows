namespace Paint.Command
{
    public class MouseEditor
    {
        private Command command;
        public MouseEditor() { }
        public void SetCommand(Command command) { this.command = command; }
        public void ExecuteCommand()
        {
            this.command.Execute();
        }
    }
}
