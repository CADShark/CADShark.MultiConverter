using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace CADShark.Common.MultiConverter
{
    internal class ConvertStep(ISldWorks swApp, string filePath)
    {
        private static readonly CadLogger Logger = CadLogger.GetLogger<ConvertStep>();

        internal void ExportFile()
        {
            ReadOptions();
            var swModel = (ModelDoc2)swApp.ActiveDoc;

            var error = 0;
            var warning = 0;
            var status = false;

            var swModelEx = swModel.Extension;

            if (swModelEx != null)
                status = swModelEx.SaveAs3(filePath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                    (int)swSaveAsOptions_e.swSaveAsOptions_Silent, null, null, ref error,
                    ref warning);
            Logger.Info($"Export STEP: {status}");
            Logger.Error($"Error: {error}");
            Logger.Warning($"Warning: {warning}");
        }

        private void ReadOptions()
        {
            swApp.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swStepAP);
        }
    }
}