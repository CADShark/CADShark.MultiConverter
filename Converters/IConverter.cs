using SolidWorks.Interop.sldworks;

namespace CADShark.Common.MultiConverter.Converters
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
    }
}
