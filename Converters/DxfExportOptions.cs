namespace CADShark.Common.MultiConverter.Converters
{
    /// <summary>
    /// Options for exporting to DXF/DWG format.
    /// </summary>
    public class DxfExportOptions
    {
        /// <summary>
        /// Gets or sets whether to export geometry.
        /// </summary>
        public bool ExportGeometry { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to include hidden edges.
        /// </summary>
        public bool IncludeHiddenEdges { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to export bend lines.
        /// </summary>
        public bool ExportBendLines { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to include sketches.
        /// </summary>
        public bool IncludeSketches { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to merge coplanar faces.
        /// </summary>
        public bool MergeCoplanarFaces { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to export library features.
        /// </summary>
        public bool ExportLibraryFeatures { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to export forming tools.
        /// </summary>
        public bool ExportFormingTools { get; set; } = false;

        /// <summary>
        /// Computes the bitwise options value for SolidWorks API.
        /// </summary>
        /// <returns>The computed options integer.</returns>
        public int ToInt()
        {
            return SheetMetalOptions(
                ExportGeometry ? 1 : 0,
                IncludeHiddenEdges ? 1 : 0,
                ExportBendLines ? 1 : 0,
                IncludeSketches ? 1 : 0,
                MergeCoplanarFaces ? 1 : 0,
                ExportLibraryFeatures ? 1 : 0,
                ExportFormingTools ? 1 : 0);
        }

        private static int SheetMetalOptions(int p0, int p1, int p2, int p3, int p4, int p5, int p6)
        {
            return p0 + p1 * 2 + p2 * 4 + p3 * 8 + p4 * 16 + p5 * 32 + p6 * 64;
        }
    }
}
