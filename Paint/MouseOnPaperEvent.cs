using BaseShapes;

namespace Paint
{
    class MouseOnPaperEvent
    {
        private readonly List<BaseShape> prototype;
        private ICommand createShapeCommand;
        public ICommand CreateShapeCommand { get { return createShapeCommand; } set { createShapeCommand = value; } }


    }
}
