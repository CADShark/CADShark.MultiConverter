using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADShark.Common.MultiConverter.CutListExtractor
{
    /// <summary>
    /// Represents data for a single cut-list item from a sheet metal part.
    /// </summary>
    internal class CutListItem
    {
        public string FolderName { get; set; }
        public int Quantity { get; set; }
        public Body2 RepresentativeBody { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; } = new Dictionary<string, string>();
    }
}
