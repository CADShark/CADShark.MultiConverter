using System;
using System.Collections.Generic;
using System.Diagnostics;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace CADShark.Common.MultiConverter.CutListExtractor
{
    /// <summary>
    /// Class to extract and store cut-list data from a SolidWorks sheet metal part using the 2025 API.
    /// Necessary parameters: ISldWorks application and ModelDoc2 document.
    /// Usage: Instantiate, call LoadCutList() to populate CutListItems for further use.
    /// </summary>
    public class SheetMetalCutListExtractor
    {
        private readonly ISldWorks _swApp;
        private readonly ModelDoc2 _swModel;
        public List<CutListItem> CutListItems { get; private set; } = new List<CutListItem>();

        /// <summary>
        /// Constructor: Pass the SolidWorks application and the active sheet metal model document.
        /// </summary>
        /// <param name="swApp">ISldWorks instance.</param>
        /// <param name="swModel">ModelDoc2 of the sheet metal part.</param>
        public SheetMetalCutListExtractor(ISldWorks swApp, ModelDoc2 swModel)
        {
            _swApp = swApp ?? throw new ArgumentNullException(nameof(swApp));
            _swModel = swModel ?? throw new ArgumentNullException(nameof(swModel));
            if (_swModel.GetType() != (int)swDocumentTypes_e.swDocPART)
            {
                throw new ArgumentException("Model must be a part document.", nameof(swModel));
            }
        }

        /// <summary>
        /// Extracts the cut-list from the sheet metal part, populating CutListItems.
        /// Traverses features to find CutListFolder features under body folders.
        /// Stores folder name, quantity (body count), representative body, and custom properties.
        /// Returns true if at least one cut-list item was found.
        /// </summary>
        public bool LoadCutList()
        {
            CutListItems.Clear();
            Feature rootFeat = _swModel.FirstFeature();
            if (rootFeat == null) return false;

            TraverseFeatures(rootFeat, true, "Root Feature");
            return CutListItems.Count > 0;
        }

        private void TraverseFeatures(Feature thisFeat, bool isTopLevel, string parentName)
        {
            Feature curFeat = thisFeat;
            while (curFeat != null)
            {
                ProcessFeature(curFeat, parentName);
                Feature subFeat = curFeat.GetFirstSubFeature();
                while (subFeat != null)
                {
                    TraverseFeatures(subFeat, false, curFeat.Name);
                    subFeat = subFeat.GetNextSubFeature();
                }
                if (isTopLevel)
                {
                    curFeat = curFeat.GetNextFeature();
                }
                else
                {
                    curFeat = null;
                }
            }
        }

        private void ProcessFeature(Feature thisFeat, string parentName)
        {
            string featType = thisFeat.GetTypeName();
            bool isBodyFolder = featType == "SolidBodyFolder" || featType == "CutListFolder" || featType == "SurfaceBodyFolder";

            if (isBodyFolder && featType == "CutListFolder")
            {
                BodyFolder bodyFolder = (BodyFolder)thisFeat.GetSpecificFeature2();
                int bodyCount = bodyFolder.GetBodyCount();
                if (bodyCount > 0)
                {
                    // This is a valid cut-list folder
                    object[] bodies = (object[])bodyFolder.GetBodies();
                    if (bodies != null && bodies.Length > 0)
                    {
                        Body2 repBody = (Body2)bodies[0]; // Representative body (identical bodies are grouped)

                        // Get custom properties from the cut-list folder feature
                        Dictionary<string, string> props = GetCustomProperties(thisFeat);

                        CutListItems.Add(new CutListItem
                        {
                            FolderName = thisFeat.Name,
                            Quantity = bodyCount,
                            RepresentativeBody = repBody,
                            CustomProperties = props
                        });
                    }
                }
            }
            else if (isBodyFolder)
            {
                // For SolidBodyFolder, set flag to process sub cut-lists
                // (Logic simplified; in full impl, track if in body folder context)
            }
        }

        private Dictionary<string, string> GetCustomProperties(Feature feat)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            CustomPropertyManager cpm = feat.CustomPropertyManager;
            if (cpm == null) return props;

            string[] propNames = (string[])cpm.GetNames();
            if (propNames == null) return props;

            foreach (string propName in propNames)
            {
                string val = "";
                string resolvedVal = "";
                bool status = cpm.Get2(propName, out val, out resolvedVal);
                if (status)
                {
                    props[propName] = resolvedVal ?? val; // Prefer resolved value
                }
            }
            return props;
        }
    }
}