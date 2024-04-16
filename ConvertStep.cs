using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace CADShark.Common.MultiConverter
{
    internal class ConvertStep : ConvertBuilder
    {
        private static readonly CadLogger Logger = CadLogger.GetLogger(className: nameof(ConvertStep));

        internal static void ExportFile()
        {
            ReadOptions();
            var swModel = (ModelDoc2)SwApp.ActiveDoc;

            var error = 0;
            var warning = 0;
            var status = false;

            var swModelEx = swModel.Extension;

            if (swModelEx != null)
                status = swModelEx.SaveAs3(FilePath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, null, ref error,
                    ref warning);
            Logger.Info($"Export STEP: {status}");
            Logger.Error($"Error: {error}");
            Logger.Warning($"Warning: {warning}");
        }

        private static void ReadOptions()
        {
            SwApp.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swStepAP);
        }
    }
}