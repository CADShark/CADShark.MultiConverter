using CADShark.Common.MultiConverter.Core;

namespace CADShark.Common.MultiConverter
{
    /// <summary>
    /// Interface for building file paths for exported files.
    /// </summary>
    public interface IFilePathBuilder
    {
        /// <summary>
        /// Builds the output file path based on the source path, export format, configuration, and optional new save path.
        /// </summary>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="format">The export format (e.g., Pdf, Dxf, Step).</param>
        /// <param name="config">Optional configuration name to include in the file name.</param>
        /// <param name="newSavePath">Optional new save path; defaults to a subfolder in the source directory.</param>
        /// <returns>The built file path.</returns>
        string Build(string sourcePath, ExportFormat format, string config = null, string newSavePath = null);
    }
}