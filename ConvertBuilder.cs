using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.IO;

namespace CADShark.Common.MultiConverter
{
    public class ConvertBuilder
    {
        private static readonly CadLogger Logger = CadLogger.GetLogger(className: nameof(ConvertBuilder));

        internal static SldWorks SwApp;
        public static string FilePath;
        

        public ConvertBuilder()
        {
        }

        public ConvertBuilder(SldWorks swApp)
        {
            SwApp = swApp;
        }

        public void ConvertToPdf()
        {
            ConvertPdf.ExportFile();
        }

        public void ConvertToDxf(out byte[] dxfByteCode, bool isSheetmetal)
        {
            ConvertDxf.ExportFile(out dxfByteCode, isSheetmetal);
        }

        public void ConvertToStep()
        {
            ConvertStep.ExportFile();
        }

        public bool IsSheetMetalComponent()
        {
            var swModel = (ModelDoc2)SwApp.ActiveDoc;

            var status = false;
            if (!(swModel is IPartDoc swPart)) return false;

            var vBodies = (object[])swPart.GetBodies2(0, false);

            if (vBodies == null) return false;

            for (var i = 0; i < vBodies.Length; i++)
            {
                var swBody = (Body2)vBodies[i];
                status = swBody.IsSheetMetal();
            }

            return status;
        }

        public bool IsSheetMetalComponent(Component2 component)
        {
            try
            {
                var status = false;
                var vBodies = (object[])component.GetBodies3((int)swBodyType_e.swSolidBody, out _);

                if (vBodies == null) return false;

                for (var i = 0; i < vBodies.Length; i++)
                {
                    var swBody = (Body2)vBodies[i];
                    status = swBody.IsSheetMetal();
                }

                return status;
            }
            catch (Exception e)
            {
                Logger.Error($"IsSheetMetalComponent (Component2) {e.Message}");
                throw;
            }
        }

        public void PathBuilder(string path, string extension, string config = null, string newSavePath = null)
        {
            var directoryName = Path.GetDirectoryName(path);

            var fileName = Path.GetFileNameWithoutExtension(path);

            var folderToSaveStep = newSavePath == null
                ? Path.Combine(directoryName, extension.ToUpper())
                : newSavePath;

            var fullName = config != null ? $"{fileName}-{config}.{extension}" : $"{fileName}.{extension}";

            if (!Directory.Exists(folderToSaveStep))
            {
                Directory.CreateDirectory(folderToSaveStep);
            }

            FilePath = Path.Combine(folderToSaveStep, fullName);

        }
    }
}