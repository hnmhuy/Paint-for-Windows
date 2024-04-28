using BaseShapes;

namespace Paint.Command
{
    public abstract class Command
    {
        protected BaseShape backup;
        public abstract void Execute();
        public abstract void Undo();
    }
}
