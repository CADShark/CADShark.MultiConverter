using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;

namespace CADShark.Common.MultiConverter.Extractors;

public class CutListExtractorService
{
    private static CustomPropertyManager _propMgr;

    public List<CutListItem> ExtractCutLists(ModelDoc2 model)
    {
        var results = new List<CutListItem>();
        if (model == null)
            return results;

        if (model is not PartDoc part)
            return results;

        var configName = model.ConfigurationManager.ActiveConfiguration.Name;
        var filePath = model.GetPathName();
        model.ForceRebuild3(false);

        Feature feat = null;
        try
        {
            feat = (Feature)part.FirstFeature();
            while (feat != null)
                try
                {
                    var typeName = feat.GetTypeName2();
                    var bodyFolder = feat.GetSpecificFeature2() as BodyFolder;
                    if (bodyFolder == null)
                        continue;
                    bodyFolder.SetAutomaticCutList(true);
                    bodyFolder.UpdateCutList();

                    if (typeName is "CutListFolder")
                        try
                        {
                            _propMgr = feat.CustomPropertyManager;

                            var item = new CutListItem
                            {
                                FilePath = filePath,
                                ConfigurationName = configName,
                                WorkpieceX =
                                    ParseDecimalProperty(
                                        GetProperties("Bounding Box Length")),
                                WorkpieceY =
                                    ParseDecimalProperty(GetProperties("Bounding Box Width")),
                                Bend = Convert.ToInt32(GetProperties("Bends")),
                                Thickness = ParseDecimalProperty(GetProperties("Sheet Metal Thickness")),
                                SurfaceArea = GetSurfaceArea(model)
                            };

                            results.Add(item);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"{e.StackTrace}");
                        }
                        finally
                        {
                            Marshal.FinalReleaseComObject(bodyFolder);
                        }
                }
                finally
                {
                    var nextFeat = feat.GetNextFeature();
                    Marshal.FinalReleaseComObject(feat);
                    feat = (Feature)nextFeat;
                }
        }
        finally
        {
            if (feat != null)
                Marshal.FinalReleaseComObject(feat);
        }

        return results;
    }

    private decimal ParseDecimalProperty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        if (decimal.TryParse(value, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var result))
            return result;

        return 0;
    }

    private static decimal GetSurfaceArea(ModelDoc2 model)
    {
        var massProp = (MassProperty2)model.Extension.CreateMassProperty2();
        return Convert.ToDecimal(massProp.SurfaceArea);
    }

    private static string GetProperties(string propName)
    {
        _propMgr.Get6(propName, false, out _, out var resolvedVal, out _, out _);

        return resolvedVal;
    }
}