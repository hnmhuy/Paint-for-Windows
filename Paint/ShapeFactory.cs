using BaseShapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public class ShapeFactory
    {

        public static BaseShape CreateShape(List<Type> types, string name, BinaryReader reader)
        {
            BaseShape? shape = null;
            // Find the type of the shape from types list
            Type type = types.FirstOrDefault(t => t.Name == name);

            if (type != null)
            {
                shape = (BaseShape)Activator.CreateInstance(type);
                shape.Load(reader);
            }

            return shape;
        }
    }
}
