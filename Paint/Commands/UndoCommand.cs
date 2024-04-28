namespace Paint.Commands
{
    public class UndoCommand : Command
    {
        private CommandHistory commandHistory;

        public UndoCommand(CommandHistory commandHistory)
        {
            this.commandHistory = commandHistory;
        }

        public override void Execute()
        {
            commandHistory.RestorePreviousCommand();
        }

        public override void Undo()
        {
            commandHistory.RestoreNextCommand();
        }
    }
}
