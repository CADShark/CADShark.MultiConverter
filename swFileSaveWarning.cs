using SolidWorks.Interop.swconst;
using System.Collections.Generic;

namespace BatchConvertToPDF
{
    internal class SwFileSaveWarning
    {
        internal static string ParseSaveError(swFileSaveWarning_e warning)
        {
            var warnings = new List<string>();

            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_AnimatorCameraViews))
            {
                warnings.Add("Animator Camera Views");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_AnimatorFeatureEdits))
            {
                warnings.Add("Animator Feature Edits");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_AnimatorLightEdits))
            {
                warnings.Add("Animator Light Edits");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_AnimatorNeedToSolve))
            {
                warnings.Add("Animator Need To Solve");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_AnimatorSectionViews))
            {
                warnings.Add("Animator Section Views");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_EdrwingsBadSelection))
            {
                warnings.Add("Edrwings Bad Selection");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_MissingOLEObjects))
            {
                warnings.Add("Missing OLE Objects");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_NeedsRebuild))
            {
                warnings.Add("Needs Rebuild");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_OpenedViewOnly))
            {
                warnings.Add("Opened View Only");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_RebuildError))
            {
                warnings.Add("Rebuild Error");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_ViewsNeedUpdate))
            {
                warnings.Add("Views Need Update");
            }
            if (warning.HasFlag(swFileSaveWarning_e.swFileSaveWarning_XmlInvalid))
            {
                warnings.Add("Xml Invalid");
            }

            return string.Join("; ", warnings);
        }
    }
}
