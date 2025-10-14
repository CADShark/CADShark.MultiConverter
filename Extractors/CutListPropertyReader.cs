using SolidWorks.Interop.sldworks;
using System.Collections.Generic;

namespace CADShark.Common.MultiConverter.Extractors
{
    public class CutListPropertyReader
    {
        public Dictionary<string, string> GetProperties(CustomPropertyManager propertyManager)
        {
            var result = new Dictionary<string, string>();

            if (propertyManager == null)
                return result;

            string[] propNames = propertyManager.GetNames() as string[];
            if (propNames == null)
                return result;

            foreach (var propName in propNames)
            {
                string valOut;
                string resolvedVal;
                bool wasResolved;
                propertyManager.Get5(propName, false, out valOut, out resolvedVal, out wasResolved);
                result[propName] = resolvedVal ?? valOut ?? string.Empty;
            }

            return result;
        }
    }
}
