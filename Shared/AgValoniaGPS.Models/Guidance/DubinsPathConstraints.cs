using AgValoniaGPS.Models;

namespace AgValoniaGPS.Models
{
    public class DubinsPathConstraints
    {

        public DubinsPathConstraints(GeoCoordDir startConstraint, GeoCoordDir goalConstraint, double radiusConstraint)
        {
            StartConstraint = startConstraint;
            GoalConstraint = goalConstraint;
            RadiusConstraint = radiusConstraint;
        }

        public GeoCoordDir StartConstraint { get; }
        public GeoCoordDir GoalConstraint { get; }
        public double RadiusConstraint { get; }
    }
}
