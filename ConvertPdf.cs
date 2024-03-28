using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;

namespace CADShark.Common.MultiConverter
{
    internal class ConvertPdf : ConvertBuilder
    {
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

    }
}