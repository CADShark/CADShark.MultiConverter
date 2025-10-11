using CADShark.Common.MultiConverter.Converters;

namespace CADShark.Common.MultiConverter
{
    /// <summary>
    /// Factory interface for creating converters based on export format.
    /// </summary>
    public interface IConverterFactory
    {
        /// <summary>
        /// Creates a converter for the specified format.
        /// </summary>
        /// <param name="format">The export format.</param>
        /// <returns>An instance of IConverter.</returns>
        IConverter Create(ExportFormat format);
    }

    /// <summary>
    /// Enumeration of supported export formats.
    /// </summary>
    public enum ExportFormat { Pdf, Dxf, Step }
}