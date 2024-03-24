using System;
using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.IO;
using CADShark.MultiConverter.sln;

namespace CADShark.Common.MultiConverter
{
    internal class ConvertPdf : ConvertBuilder
    {
        private static string _filepath;
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
                _filepath = swModel.GetPathName();

                //_helper.SuppressUpdates(false);

                var pdfPath = GetPath();

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

                boolStatus = swModel.Extension.SaveAs3(pdfPath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)swSaveAsOptions_e.swSaveAsOptions_UpdateInactiveViews, null, null, errors, warnings);

                Logger.Info($"SaveAs Status: {boolStatus}");
            }
            catch (Exception e)
            {
                Logger.Info($"SaveAs Status: {e.Message}");
                throw;
            }
        }

        private static string GetPath()
        {
            var folderToSavePdf = Path.GetDirectoryName(_filepath);
            // ReSharper disable once AssignNullToNotNullAttribute
            var fullPath = Path.Combine(folderToSavePdf, "PDF");
            Directory.CreateDirectory(fullPath);

            var pdfName = Path.GetFileName(_filepath).Replace(".SLDDRW", ".PDF");
            var pdfPath = Path.Combine(fullPath, pdfName);

            return pdfPath;
        }
    }
}