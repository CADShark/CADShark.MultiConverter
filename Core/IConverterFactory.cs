namespace CADShark.Common.MultiConverter.Core
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
}