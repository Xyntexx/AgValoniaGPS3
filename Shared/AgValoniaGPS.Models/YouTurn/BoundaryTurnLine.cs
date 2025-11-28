using AgValoniaGPS.Models.Base;
using System.Collections.Generic;

namespace AgValoniaGPS.Models.YouTurn
{
    /// <summary>
    /// Represents a boundary turn line used for U-turn creation.
    /// </summary>
    public class BoundaryTurnLine
    {
        /// <summary>
        /// Points defining the turn line boundary.
        /// </summary>
        public List<Vec3> Points { get; set; } = new List<Vec3>();

        /// <summary>
        /// Index of this boundary in the boundary list.
        /// </summary>
        public int BoundaryIndex { get; set; }
    }
}
