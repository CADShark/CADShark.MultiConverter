using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;
using View = SolidWorks.Interop.sldworks.View;

namespace CADShark.Common.MultiConverter.Converters;

/// <summary>
/// Converter for exporting SolidWorks drawings to PDF format.
/// </summary>
public class PdfConverter(ISldWorks swApp) : BaseConverter(swApp)
{
    private static readonly CadLogger Logger = CadLogger.GetLogger<PdfConverter>();

    /// <summary>
    /// Resolves all views in the drawing to full resolution.
    /// </summary>
    /// <param name="swDrawDoc">The drawing document.</param>
    private static void ResolveAllViews(DrawingDoc swDrawDoc)
    {
        var swView = swDrawDoc.GetFirstView() as View;
        while (swView != null)
        {
            swView.SetLightweightToResolved();
            swView = swView.GetNextView() as View;
        }
    }

    /// <summary>
    /// Performs the PDF export.
    /// </summary>
    /// <param name="model">The SolidWorks model document to export.</param>
    /// <param name="path">The file path to save the exported PDF file.</param>
    /// <returns>True if the export was successful; otherwise, false.</returns>
    protected override bool DoExport(ModelDoc2 model, string path)
    {
        bool status;
        if (model is not DrawingDoc swDrawDoc) throw new ArgumentException("Model must be a drawing");

        var sheets = swDrawDoc.GetSheetNames();


        foreach (var sheetName in (string[])sheets)
        {
            status = swDrawDoc.ActivateSheet(sheetName);

            Logger.Debug($@"Activates the specified drawing sheet: Sheet name: {sheetName}. Status: {status}");
            if (!status) Logger.Error($@"Activates the specified drawing sheet: Sheet name: {sheetName}. Status: {status}");

            ResolveAllViews(swDrawDoc);
        }
        model.EditRebuild3();
        swDrawDoc.ForceRebuild();

        var swExportPdfData = (ExportPdfData)SwApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);


        status = swExportPdfData.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportAllSheets, sheets);
        
        Logger.Debug($@"Sets the drawing sheets to export. Status: {status}");
        if (!status) Logger.Error($@"Sets the drawing sheets to export.  Status: {status}");
        
        swExportPdfData.ViewPdfAfterSaving = false;

        var errors = -1;
        var warnings = -1;

        status = model.Extension.SaveAs3(path,
            (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
            (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
            swExportPdfData,
            null,
            ref errors,
            ref warnings);

        if (!status)
        {
            var errorDesc = SwSaveOperation.ParseSaveError((swFileSaveError_e)errors);
            Logger.Error(
                "Failed to export PDF {FilePath}. ErrorCode={ErrorCode}, Description={Description}",
                path, errors, errorDesc
            );
        }
        else
        {
            Logger.Info($"PDF exported successfully:{path}");
            if (warnings != 0) Logger.Warning("PDF export warning: {warnings}", warnings);
        }

        return status;
    }
}

internal static class SwFileSaveWarning
{
    // mapping of flag -> text
    private static readonly (swFileSaveWarning_e Flag, string Text)[] s_map =
    {
        (swFileSaveWarning_e.swFileSaveWarning_AnimatorCameraViews, "Animator Camera Views"),
        (swFileSaveWarning_e.swFileSaveWarning_AnimatorFeatureEdits, "Animator Feature Edits"),
        (swFileSaveWarning_e.swFileSaveWarning_AnimatorLightEdits, "Animator Light Edits"),
        (swFileSaveWarning_e.swFileSaveWarning_AnimatorNeedToSolve, "Animator Need To Solve"),
        (swFileSaveWarning_e.swFileSaveWarning_AnimatorSectionViews, "Animator Section Views"),
        (swFileSaveWarning_e.swFileSaveWarning_EdrwingsBadSelection, "Edrwings Bad Selection"),
        (swFileSaveWarning_e.swFileSaveWarning_MissingOLEObjects, "Missing OLE Objects"),
        (swFileSaveWarning_e.swFileSaveWarning_NeedsRebuild, "Needs Rebuild"),
        (swFileSaveWarning_e.swFileSaveWarning_OpenedViewOnly, "Opened View Only"),
        (swFileSaveWarning_e.swFileSaveWarning_RebuildError, "Rebuild Error"),
        (swFileSaveWarning_e.swFileSaveWarning_ViewsNeedUpdate, "Views Need Update"),
        (swFileSaveWarning_e.swFileSaveWarning_XmlInvalid, "Xml Invalid"),
    };

    private static readonly ConcurrentDictionary<swFileSaveWarning_e, string> s_cache = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ParseSaveError(swFileSaveWarning_e warning)
    {
        if (warning == 0)
            return string.Empty;

        // cache lookup
        if (s_cache.TryGetValue(warning, out var cached))
            return cached;

        var sb = new StringBuilder(64);
        bool first = true;

        foreach (var (flag, text) in s_map)
        {
            if ((warning & flag) != 0)
            {
                if (!first) sb.Append("; ");
                first = false;
                sb.Append(text);
            }
        }

        var result = sb.ToString();
        s_cache.TryAdd(warning, result);
        return result;
    }
}