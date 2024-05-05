namespace Paint.Commands
{
    public class CommandHistory
    {
        private List<Command> historyCommands = new List<Command>();
        private int currentCommandIndex = -1;
        private static int MAX_HISTORY_SIZE = 100;
        public CommandHistory() { }

        public void AddCoomand(Command command)
        {
            // Checking the size of history
            if (historyCommands.Count >= MAX_HISTORY_SIZE)
            {
                historyCommands.RemoveAt(0);
                currentCommandIndex--;
            }
            // Checking if the current command is not the last command in the history
            if (currentCommandIndex < historyCommands.Count - 1)
            {
                // Removing all the commands after the current command
                historyCommands.RemoveRange(currentCommandIndex + 1, historyCommands.Count - currentCommandIndex - 1);
            }
            historyCommands.Add(command);
            currentCommandIndex++;

        }

        public bool CanUndo()
        {
            return currentCommandIndex >= 0;
        }

        public bool CanRedo()
        {
            return currentCommandIndex < historyCommands.Count - 1;
        }

        public void RestorePreviousCommand()
        {
            if (currentCommandIndex >= 0)
            {
                historyCommands[currentCommandIndex].Undo();
                currentCommandIndex--;
            }
        }

        public void RestoreNextCommand()
        {
            if (currentCommandIndex < historyCommands.Count - 1)
            {
                currentCommandIndex++;
                historyCommands[currentCommandIndex].Execute();
            }
        }

        public void Pop()
        {
            historyCommands.RemoveAt(currentCommandIndex);
            currentCommandIndex--;
        }

        public void Clear()
        {
            historyCommands.Clear();
            currentCommandIndex = -1;
        }

    }
}
