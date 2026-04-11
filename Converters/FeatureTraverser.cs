using SolidWorks.Interop.sldworks;

namespace CADShark.Common.MultiConverter.Converters;

public static class FeatureTraverser
{
    private static ModelDoc2 _swModel;


    public static void UnsuppressFlatPatternFeatures(ModelDoc2 swModel)
    {
        _swModel = swModel;
        var swFeat = (Feature)_swModel.FirstFeature();
        TraverseFeatureFeatures(swFeat);
    }

    private static void TraverseFeatureFeatures(Feature swFeat)
    {
        while (swFeat != null)
        {
            var featType = swFeat.GetTypeName2();

            if (featType == "FlatPattern") TraverseSubFeatures(swFeat);

            swFeat = (Feature)swFeat.GetNextFeature();
        }
    }

    private static void TraverseSubFeatures(Feature parentFeature)
    {
        var subFeat = (Feature)parentFeature.GetFirstSubFeature();

        while (subFeat != null)
        {
            var name = subFeat.Name;
            var type = subFeat.GetTypeName2();

            if (type == "UiBend")
            {
                SelectAndUnsuppress(name, "BODYFEATURE");

                TraverseThirdLevel(subFeat);
            }
            else
            {
                SelectAndUnsuppress(name, "SKETCH");
            }

            subFeat = (Feature)subFeat.GetNextSubFeature();
        }
    }

    private static void TraverseThirdLevel(Feature feature)
    {
        var subSubFeat = (Feature)feature.GetFirstSubFeature();

        while (subSubFeat != null)
        {
            var subSubSubFeat = (Feature)subSubFeat.GetFirstSubFeature();

            while (subSubSubFeat != null) subSubSubFeat = (Feature)subSubSubFeat.GetNextSubFeature();

            subSubFeat = (Feature)subSubFeat.GetNextSubFeature();
        }
    }

    private static void SelectAndUnsuppress(string name, string type)
    {
        if (string.IsNullOrEmpty(name)) return;

        var status = _swModel.Extension.SelectByID2(
            name,
            type,
            0, 0, 0,
            false,
            0,
            null,
            0);

        if (!status) return;
        _swModel.EditUnsuppress2();
        _swModel.ClearSelection2(true);
    }
}