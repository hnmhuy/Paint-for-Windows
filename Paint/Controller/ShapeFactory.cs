using BaseShapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paint.Controller
{
    public class ShapeFactory
    {

        public static BaseShape CreateShape(List<BaseShape> prototypes, string name, BinaryReader reader)
        {
            BaseShape shape = null;
            foreach (BaseShape prototype in prototypes)
            {
                if (prototype.Name == name)
                {
                    shape = (BaseShape)prototype.Clone();
                    shape.Load(reader);
                    shape.Render();
                    break;
                }
            }
            return shape;
        }
    }
}
