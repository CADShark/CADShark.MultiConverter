using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.IO;


namespace CADShark.Common.MultiConverter;

/// <summary>
/// Main class for converting SolidWorks files
/// </summary>
public class ConvertBuilder(ISldWorks swApp)
{
    private static readonly CadLogger Logger = CadLogger.GetLogger<ConvertBuilder>();
    public string FilePath { get; private set; }

    public void ConvertToPdf(ModelDoc2 swModel)
    {
        var pdfConverter = new ConvertPdf(swApp, FilePath);
        pdfConverter.Export(swModel);
    }

    public void ConvertToDxf(bool isSheetMetal)
    {
        var dxfConverter = new ConvertDxf(swApp, FilePath);
        dxfConverter.ExportFile(isSheetMetal);
    }

    public void ConvertToStep()
    {
        var stepConverter = new ConvertStep(swApp, FilePath);
        stepConverter.ExportFile();
    }

    public bool IsSheetMetalComponent()
    {
        var swModel = (ModelDoc2)swApp.ActiveDoc;
        var status = false;

        if (!(swModel is IPartDoc swPart)) return false;

        var vBodies = (object[])swPart.GetBodies2(0, false);

        if (vBodies == null) return false;

        foreach (var body in vBodies)
        {
            var swBody = (Body2)body;
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

    public void BuildFilePath(string path, string extension, string config = null, string newSavePath = null)
    {
        var directoryName = Path.GetDirectoryName(path);

        var fileName = Path.GetFileNameWithoutExtension(path);

        if (directoryName == null) return;
        var folderToSaveStep = newSavePath ?? Path.Combine(directoryName, extension.ToUpper());

        var fullName = config != null ? $"{fileName}-{config}.{extension}" : $"{fileName}.{extension}";

        if (!Directory.Exists(folderToSaveStep)) Directory.CreateDirectory(folderToSaveStep);

        FilePath = Path.Combine(folderToSaveStep, fullName);
    }
}