using CADShark.Common.Logging;
using CADShark.Common.MultiConverter.Exceptions;
//using Kompas6Constants;
//using KompasAPI7;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using View = SolidWorks.Interop.sldworks.View;

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
            _errors = -1;
            _warnings = -1;
            var swModel = (ModelDoc2)SwApp.ActiveDoc;
            var swModExt = swModel.Extension;
            var swExportPdfData =
                (ExportPdfData)SwApp.GetExportFileData((int)swExportDataFileType_e.swExportPdfData);

            Logger.Info($"Save path: {FilePath}");

            // ReSharper disable once SuspiciousTypeConversion.Global
            var swDrawDoc = (DrawingDoc)swModel;

            var obj = swDrawDoc.GetSheetNames() as string[];

            // Save the drawings sheets to a PDF file
            swExportPdfData.SetSheets((int)swExportDataSheetsToExport_e.swExportData_ExportAllSheets, obj);
            swExportPdfData.ViewPdfAfterSaving = false;
            var boolStatus = swModExt.SaveAs3(FilePath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                (int)swSaveAsOptions_e.swSaveAsOptions_Silent, swExportPdfData, null, ref _errors, ref _warnings);

            if (boolStatus) return;

            throw new SaveDocumentFailedException(_errors, SwSaveOperation.ParseSaveError((swFileSaveError_e)_errors));

            //Logger.Error($"{SwSaveOperation.ParseSaveError((swFileSaveError_e)_errors)}");
        }

        //internal static bool KompasExportFile()
        //{
        //    Document2D.SaveAs(FilePath);
        //    return Document2D.Close(DocumentCloseOptions.kdDoNotSaveChanges);
        //}
    }
}