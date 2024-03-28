using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.IO;

namespace CADShark.Common.MultiConverter
{
    internal class ConvertStep : ConvertBuilder
    {
        private static readonly CadLogger Logger = CadLogger.GetLogger(className: nameof(ConvertStep));
        private static ModelDoc2 _swModel;
        private static string _configName;
        public static string StepPath;

        internal static void ExportFile()
        {
            ReadOptions();
            _swModel = (ModelDoc2)SwApp.ActiveDoc;
            var configManager = _swModel.ConfigurationManager;
            _configName = configManager.ActiveConfiguration.Name;

            var error = 0;
            var warning = 0;
            var status = false;

            var swModelEx = _swModel.Extension;
            var filePath = CreatePath();

            if (swModelEx != null)
                status = swModelEx.SaveAs3(filePath, (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
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

        private static string CreatePath()
        {
            var filePath = string.Empty;
            var directoryName = Path.GetDirectoryName(_swModel.GetPathName());
            if (directoryName == null) return filePath;
            var folderToSaveStep = Path.Combine(directoryName, "STEP");
            var fileName = _swModel.GetTitle();
            var fullName = $"{fileName}-{_configName}.STEP";

            if (!Directory.Exists(folderToSaveStep))
            {
                Directory.CreateDirectory(folderToSaveStep);
            }

            filePath = Path.Combine(folderToSaveStep, fullName);

            return filePath;
        }
    }
}