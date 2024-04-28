
using BaseShapes;
using System.Windows.Controls;

namespace ShapeEllipse
{
    public class ShapeEllipse : BaseShape
    {
        public ShapeEllipse()
        {
            _name = "Ellipse";
            _iconName = "rectangle.png";
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override Canvas Render()
        {
            throw new NotImplementedException();
        }

        public override void Resize()
        {
            throw new NotImplementedException();
        }
    }

}
