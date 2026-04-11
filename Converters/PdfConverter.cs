//using CADShark.Common.Logging;

using CADShark.Common.MultiConverter.Exceptions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using View = SolidWorks.Interop.sldworks.View;

namespace CADShark.Common.MultiConverter.Converters;

/// <summary>
/// Converter for exporting SolidWorks drawings to PDF format.
/// </summary>
public class PdfConverter(ISldWorks swApp) : BaseConverter(swApp)
{
    //private new static readonly CadLogger Logger = CadLogger.GetLogger<PdfConverter>();

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
    /// <param name="config"></param>
    /// <returns>True if the export was successful; otherwise, false.</returns>
    protected override bool DoExport(ModelDoc2 model, string path, string config)
    {
        bool status;
        if (model is not DrawingDoc swDrawDoc) throw new ArgumentException("Model must be a drawing");

        var sheets = swDrawDoc.GetSheetNames();


        foreach (var sheetName in (string[])sheets)
        {
            status = swDrawDoc.ActivateSheet(sheetName);

            //Logger.Debug($@"Activates the specified drawing sheet: Sheet name: {sheetName}. Status: {status}");
            //if (!status) Logger.Error($@"Activates the specified drawing sheet: Sheet name: {sheetName}. Status: {false}");

            ResolveAllViews(swDrawDoc);
        }

        model.EditRebuild3();
        swDrawDoc.ForceRebuild();

        var swExportPdfData = (ExportPdfData)SwApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);


        status = swExportPdfData.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportAllSheets, sheets);

        //Logger.Debug($@"Sets the drawing sheets to export. Status: {status}");
        //if (!status) Logger.Error($@"Sets the drawing sheets to export.  Status: {false}");

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
            //Logger.Error(
            //    "Failed to export PDF {FilePath}. ErrorCode={ErrorCode}, Description={Description}",
            //    path, errors, errorDesc
            //);
        }
        else
        {
            //Logger.Info($"PDF exported successfully:{path}");
            //if (warnings != 0) Logger.Warning("PDF export warning: {warnings}", warnings);
        }

        return status;
    }
}