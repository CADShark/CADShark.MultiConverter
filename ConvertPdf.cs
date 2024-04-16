using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Runtime.InteropServices;

namespace CADShark.Common.MultiConverter
{
    internal class ConvertPdf : ConvertBuilder
    {
        private static int _errors;
        private static int _warnings;
        private static readonly CadLogger Logger = CadLogger.GetLogger(className: nameof(ConvertPdf));

        internal static void ExportFile()
        {
            try
            {
                const int errors = 0;
                const int warnings = 0;
                bool boolStatus;

                var swModel = (ModelDoc2)SwApp.ActiveDoc;

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


                Logger.Trace($"Save path: {FilePath}");
                boolStatus = swModel.Extension.SaveAs3(FilePath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, null, null, errors, warnings);

                Logger.Info($"SaveAs Status: {boolStatus}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Export PDF Status is False", ex);
                throw;
            }
        }

        internal static void ExportFile2()
        {
            try
            {
                var swModel = (ModelDoc2)SwApp.ActiveDoc;
                var swModExt = swModel.Extension;
                var swExportPdfData =
                    (ExportPdfData)SwApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);

                Logger.Info($"Save path: {FilePath}");
                // Get the names of the drawing sheets in the drawing
                // to get the size of the array of drawing sheets

                // ReSharper disable once SuspiciousTypeConversion.Global
                var swDrawDoc = (DrawingDoc)swModel;

                var obj = (string[])swDrawDoc.GetSheetNames();
                var count = obj.Length;
                int i;
                var objects = new object[count - 1];
                var arrObjIn = new DispatchWrapper[count - 1];

                // Activate each drawing sheet, except the last drawing sheet, for
                // demonstration purposes only and add each sheet to an array
                // of drawing sheets
                for (i = 0; i < count - 1; i++)
                {
                    swDrawDoc.ActivateSheet((obj[i]));
                    var swSheet = (Sheet)swDrawDoc.GetCurrentSheet();
                    objects[i] = swSheet;
                    arrObjIn[i] = new DispatchWrapper(objects[i]);
                }

                // Save the drawings sheets to a PDF file
                swExportPdfData.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportSpecifiedSheets,
                    (arrObjIn));
                swExportPdfData.ViewPdfAfterSaving = false;
                var boolStatus = swModExt.SaveAs3(FilePath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)swSaveAsOptions_e.swSaveAsOptions_Silent, swExportPdfData, null, ref _errors, ref _warnings);

                if (_errors > 0)
                {
                    Logger.Error($"{_errors}");
                    boolStatus = false;
                }

                if (_warnings > 0)
                {
                    Logger.Warning($"{_warnings}");
                }

                Logger.Info($"SaveAs Status: {boolStatus}");
            }
            catch (Exception ex)
            {
                Logger.Error($"{_errors}");
                Logger.Error($"Export PDF Status is False", ex);
                throw;
            }
        }
    }
}