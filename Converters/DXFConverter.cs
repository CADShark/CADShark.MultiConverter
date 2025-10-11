using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using System;
using System.Linq;

namespace CADShark.Common.MultiConverter.Converters;

/// <summary>
/// Converter for exporting SolidWorks parts to DXF format.
/// </summary>
public class DxfConvert(ISldWorks swApp) : BaseConverter(swApp)
{
    private static readonly CadLogger Logger = CadLogger.GetLogger<DxfConvert>();

    /// <summary>
    /// Gets or sets the export options for DXF.
    /// </summary>
    public DxfExportOptions Options { get; set; } = new();

    /// <summary>
    /// Performs the DXF export.
    /// </summary>
    /// <param name="model">The SolidWorks model document to export.</param>
    /// <param name="path">The file path to save the exported DXF file.</param>
    /// <returns>True if the export was successful; otherwise, false.</returns>
    protected override bool DoExport(ModelDoc2 model, string path)
    {
        if (model is not IPartDoc partDoc)
        {
            Logger.Warning("Model is not a PartDoc");
            return false;
        }

        var configManager = model.ConfigurationManager;
        var configName = configManager.ActiveConfiguration.Name;

        var sheetMetal = IsSheetMetalComponent(model);

        try
        {
            object alignment = new[]
            {
                0.0,
                0.0,
                0.0,
                1.0,
                0.0,
                0.0,
                0.0,
                1.0,
                0.0,
                0.0,
                0.0,
                1.0
            };
            var options = Options.ToInt();
            var modelName = model.GetPathName();
            var action = sheetMetal ? 1 : 2;
            object views = sheetMetal ? 0 : 3;

            Logger.Debug($"Path export: {path}");

            var status = partDoc.ExportToDWG2(path, modelName, action, true, alignment, false, false, options, views);

            Logger.Debug($"Status export: {status} Configuration: {configName}");
            return status;
        }
        catch (Exception ex)
        {
            var error = $"Failed build dxf {model.GetTitle()} with configuration {configName} {ex.Message}";
            Logger.Error(error);
            return false;
        }
    }

    /// <summary>
    /// Checks if the model contains at least one sheet metal body.
    /// </summary>
    /// <param name="model">The SolidWorks model document to check.</param>
    /// <returns>True if at least one body is sheet metal; otherwise, false.</returns>
    public bool IsSheetMetalComponent(ModelDoc2 model)
    {
        if (model is not IPartDoc swPart) return false;

        var vBodies = (object[])swPart.GetBodies2(0, false);

        if (vBodies == null || vBodies.Length == 0) return false;

        return vBodies.Cast<Body2>().Any(swBody => swBody.IsSheetMetal());
    }
}