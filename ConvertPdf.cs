using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using View = SolidWorks.Interop.sldworks.View;

namespace CADShark.Common.MultiConverter;

internal class ConvertPdf(ISldWorks swApp, string filePath)
{

    private static readonly CadLogger Logger = CadLogger.GetLogger<ConvertPdf>();

    internal void ExportFile()
    {
        try
        {
            const int errors = 0;
            const int warnings = 0;
            bool boolStatus;

            var swModel = (ModelDoc2)swApp.ActiveDoc;

            Logger.Trace($"ExportFile {swModel.GetTitle()}");

            // ReSharper disable once SuspiciousTypeConversion.Global
            var swDrawDoc = (DrawingDoc)swModel;
            var sheetNames = (string[])swDrawDoc.GetSheetNames();

            foreach (var name in sheetNames)
            {
                boolStatus = swDrawDoc.ActivateSheet(name);
                Logger.Trace($"ActivateSheet Status: {boolStatus}");
                var swView = (View)swDrawDoc.GetFirstView();
                while (swView != null)
                {
                    swView.SetLightweightToResolved();
                    swView = (View)swView.GetNextView();
                }
            }


            Logger.Trace($"Save path: {filePath}");
            boolStatus = swModel.Extension.SaveAs3(filePath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, null, null, errors, warnings);

            Logger.Info($"SaveAs Status: {boolStatus}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Export PDF Status is False", ex);
            throw;
        }
    }

    /// <summary>
    /// Экспорт текущего активного документа в PDF
    /// </summary>
    public void Export(ModelDoc2 swModel)
    {
        var swDrawDoc = (DrawingDoc)swModel;

        ResolveAllViews(swDrawDoc);

        var swExportPdfData = (ExportPdfData)swApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);
        var sheets = swDrawDoc.GetSheetNames() as string[];

        swExportPdfData.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportAllSheets, sheets);
        swExportPdfData.ViewPdfAfterSaving = false;

        var errors = -1;
        var warnings = -1;

        var status = swModel.Extension.SaveAs3(filePath,
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
                filePath, errors, errorDesc
            );
        }
        else
        {
            Logger.Info($"PDF exported successfully:{filePath}");
            if (warnings != 0) Logger.Warning("PDF export warning: {warnings}", warnings);
        }
    }

    private static void ResolveAllViews(DrawingDoc swDrawDoc)
    {
        var swView = swDrawDoc.GetFirstView() as View;
        while (swView != null)
        {
            swView.SetLightweightToResolved();
            swView = swView.GetNextView() as View;
        }
    }
}