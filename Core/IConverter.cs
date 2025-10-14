using CADShark.Common.MultiConverter.Extractors;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;

namespace CADShark.Common.MultiConverter.Core
{
    /// <summary>
    /// Interface for converters that export SolidWorks models.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Exports the model to the specified file path.
        /// </summary>
        /// <param name="model">The SolidWorks model document to export.</param>
        /// <param name="filePath">The file path to save the exported file.</param>
        /// <returns>True if the export was successful; otherwise, false.</returns>
        bool Export(ModelDoc2 model, string filePath);

        /// <summary>
        /// Extracts cut-list data for the given model.
        /// </summary>
        /// <param name="model">The SolidWorks model document.</param>
        /// <returns>List of CutListItem for the model, or empty list if none found.</returns>
        public List<CutListItem> GetCutListData(ModelDoc2 model);
    }
}
