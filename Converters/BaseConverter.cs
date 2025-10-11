using SolidWorks.Interop.sldworks;


namespace CADShark.Common.MultiConverter.Converters;

/// <summary>
/// Abstract base class for converters that export SolidWorks models to various formats.
/// </summary>
public abstract class BaseConverter(ISldWorks swApp) : IConverter
{
    /// <summary>
    /// The SolidWorks application instance.
    /// </summary>
    protected readonly ISldWorks SwApp = swApp;

    /// <summary>
    /// Exports the model to the specified path.
    /// </summary>
    /// <param name="model">The SolidWorks model document to export.</param>
    /// <param name="path">The file path to save the exported file.</param>
    /// <returns>True if the export was successful; otherwise, false.</returns>
    public bool Export(ModelDoc2 model, string path)
    {
        PrepareModel(model);
        return DoExport(model, path);
    }

    /// <summary>
    /// Prepares the model before export. Override in derived classes if needed.
    /// </summary>
    /// <param name="model">The SolidWorks model document to prepare.</param>
    protected virtual void PrepareModel(ModelDoc2 model)
    {
        /* Common preparation */
    }

    /// <summary>
    /// Performs the actual export operation. Must be implemented in derived classes.
    /// </summary>
    /// <param name="model">The SolidWorks model document to export.</param>
    /// <param name="path">The file path to save the exported file.</param>
    /// <returns>True if the export was successful; otherwise, false.</returns>
    protected abstract bool DoExport(ModelDoc2 model, string path);
}