using System;

namespace CADShark.Common.MultiConverter.Extractors
{
    /// <summary>
    /// Represents data for a single cut-list item from a sheet metal part.
    /// </summary>
    public class CutListItem
    {
        public string FilePath { get; set; }
        public string ConfigurationName { get; set; }

        public Decimal WorkpieceX { get; set; }

        public Decimal WorkpieceY { get; set; }

        public int Bend { get; set; }

        public Decimal Thickness { get; set; }

        public Decimal SurfaceArea { get; set; }
    }
}