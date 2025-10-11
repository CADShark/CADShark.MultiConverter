using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace CADShark.Common.MultiConverter.Converters;

/// <summary>
/// Converter for exporting SolidWorks models to STEP format.
/// </summary>
public class StepConverter(ISldWorks swApp) : BaseConverter(swApp)
{
    private static readonly CadLogger Logger = CadLogger.GetLogger<StepConverter>();

    /// <summary>
    /// Performs the STEP export.
    /// </summary>
    /// <param name="model">The SolidWorks model document to export.</param>
    /// <param name="path">The file path to save the exported STEP file.</param>
    /// <returns>True if the export was successful; otherwise, false.</returns>
    protected override bool DoExport(ModelDoc2 model, string path)
    {
        try
        {
            var errors = -1;
            var warnings = -1;

            var status = model.Extension.SaveAs3(path,
                (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
                null,
                null,
                ref errors,
                ref warnings);

            if (!status)
            {
                var errorDesc = SwSaveOperation.ParseSaveError((swFileSaveError_e)errors);
                Logger.Error(
                    "Failed to export STEP {FilePath}. ErrorCode={ErrorCode}, Description={Description}",
                    path, errors, errorDesc
                );
            }
            else
            {
                Logger.Info($"STEP exported successfully: {path}");
                if (warnings != 0) Logger.Warning("STEP export warning: {warnings}", warnings);
            }

            return status;
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to export STEP {model.GetTitle()}: {ex.Message}");
            return false;
        }
    }
}