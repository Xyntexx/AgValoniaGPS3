using System.Collections.Generic;
using System.Linq;
using AgValoniaGPS.Models.Base;
using Clipper2Lib;

namespace AgValoniaGPS.Services.Geometry;

/// <summary>
/// Service for creating offset polygons using Clipper2 library.
/// Used for headland generation from field boundaries.
/// </summary>
public class PolygonOffsetService : IPolygonOffsetService
{
    // Clipper2 uses integer coordinates, so we scale by this factor for precision
    private const double Scale = 1000.0;

    /// <summary>
    /// Create an inward offset polygon from a boundary.
    /// </summary>
    /// <param name="boundaryPoints">Outer boundary points (should be clockwise for outer boundary)</param>
    /// <param name="offsetDistance">Inward offset distance in meters (positive = inward/shrink)</param>
    /// <param name="joinType">How to handle corners (Round, Miter, Square)</param>
    /// <returns>Offset polygon points, or null if offset collapses the polygon</returns>
    public List<Vec2>? CreateInwardOffset(List<Vec2> boundaryPoints, double offsetDistance, OffsetJoinType joinType = OffsetJoinType.Round)
    {
        if (boundaryPoints == null || boundaryPoints.Count < 3)
            return null;

        if (offsetDistance <= 0)
            return new List<Vec2>(boundaryPoints);

        // Convert to Clipper2 path (scaled to integers)
        var path = new Path64(boundaryPoints.Count);
        foreach (var pt in boundaryPoints)
        {
            path.Add(new Point64((long)(pt.Easting * Scale), (long)(pt.Northing * Scale)));
        }

        // Create offset (negative = inward for clockwise polygon)
        var clipperOffset = new ClipperOffset();

        JoinType clipperJoinType = joinType switch
        {
            OffsetJoinType.Miter => JoinType.Miter,
            OffsetJoinType.Square => JoinType.Square,
            _ => JoinType.Round
        };

        clipperOffset.AddPath(path, clipperJoinType, EndType.Polygon);

        var solution = new Paths64();
        // Negative offset = shrink/inward
        clipperOffset.Execute(-offsetDistance * Scale, solution);

        if (solution.Count == 0 || solution[0].Count < 3)
            return null;

        // Convert back to Vec2 list
        var result = new List<Vec2>(solution[0].Count);
        foreach (var pt in solution[0])
        {
            result.Add(new Vec2(pt.X / Scale, pt.Y / Scale));
        }

        return result;
    }

    /// <summary>
    /// Create an outward offset polygon from a boundary.
    /// Used for inner boundaries (islands) where headland goes outward.
    /// </summary>
    /// <param name="boundaryPoints">Inner boundary points</param>
    /// <param name="offsetDistance">Outward offset distance in meters</param>
    /// <param name="joinType">How to handle corners</param>
    /// <returns>Offset polygon points</returns>
    public List<Vec2>? CreateOutwardOffset(List<Vec2> boundaryPoints, double offsetDistance, OffsetJoinType joinType = OffsetJoinType.Round)
    {
        if (boundaryPoints == null || boundaryPoints.Count < 3)
            return null;

        if (offsetDistance <= 0)
            return new List<Vec2>(boundaryPoints);

        var path = new Path64(boundaryPoints.Count);
        foreach (var pt in boundaryPoints)
        {
            path.Add(new Point64((long)(pt.Easting * Scale), (long)(pt.Northing * Scale)));
        }

        var clipperOffset = new ClipperOffset();

        JoinType clipperJoinType = joinType switch
        {
            OffsetJoinType.Miter => JoinType.Miter,
            OffsetJoinType.Square => JoinType.Square,
            _ => JoinType.Round
        };

        clipperOffset.AddPath(path, clipperJoinType, EndType.Polygon);

        var solution = new Paths64();
        // Positive offset = expand/outward
        clipperOffset.Execute(offsetDistance * Scale, solution);

        if (solution.Count == 0 || solution[0].Count < 3)
            return null;

        var result = new List<Vec2>(solution[0].Count);
        foreach (var pt in solution[0])
        {
            result.Add(new Vec2(pt.X / Scale, pt.Y / Scale));
        }

        return result;
    }

    /// <summary>
    /// Create multiple concentric offset polygons (for multi-pass headlands).
    /// </summary>
    /// <param name="boundaryPoints">Outer boundary points</param>
    /// <param name="offsetDistance">Distance per pass in meters</param>
    /// <param name="passes">Number of passes</param>
    /// <param name="joinType">How to handle corners</param>
    /// <returns>List of offset polygons from outermost to innermost</returns>
    public List<List<Vec2>> CreateMultiPassOffset(List<Vec2> boundaryPoints, double offsetDistance, int passes, OffsetJoinType joinType = OffsetJoinType.Round)
    {
        var result = new List<List<Vec2>>();

        if (boundaryPoints == null || boundaryPoints.Count < 3 || passes <= 0)
            return result;

        var currentBoundary = boundaryPoints;

        for (int i = 0; i < passes; i++)
        {
            var offset = CreateInwardOffset(currentBoundary, offsetDistance, joinType);
            if (offset == null || offset.Count < 3)
                break;

            result.Add(offset);
            currentBoundary = offset;
        }

        return result;
    }

    /// <summary>
    /// Create an offset of an open polyline (not closed polygon).
    /// Used for headland segments from boundary clips.
    /// </summary>
    /// <param name="linePoints">Open polyline points</param>
    /// <param name="offsetDistance">Offset distance in meters (positive = left of travel direction)</param>
    /// <param name="joinType">How to handle corners</param>
    /// <returns>Offset line points, or null if offset fails</returns>
    public List<Vec2>? CreateLineOffset(List<Vec2> linePoints, double offsetDistance, OffsetJoinType joinType = OffsetJoinType.Round)
    {
        if (linePoints == null || linePoints.Count < 2)
            return null;

        if (offsetDistance == 0)
            return new List<Vec2>(linePoints);

        // Convert to Clipper2 path (scaled to integers)
        var path = new Path64(linePoints.Count);
        foreach (var pt in linePoints)
        {
            path.Add(new Point64((long)(pt.Easting * Scale), (long)(pt.Northing * Scale)));
        }

        // Create offset for open path
        var clipperOffset = new ClipperOffset();

        JoinType clipperJoinType = joinType switch
        {
            OffsetJoinType.Miter => JoinType.Miter,
            OffsetJoinType.Square => JoinType.Square,
            _ => JoinType.Round
        };

        // EndType.Round for open path with rounded ends
        clipperOffset.AddPath(path, clipperJoinType, EndType.Round);

        var solution = new Paths64();
        clipperOffset.Execute(offsetDistance * Scale, solution);

        if (solution.Count == 0 || solution[0].Count < 2)
            return null;

        // The result is a closed polygon around the line
        // We need to extract just one side - find the points closest to our original offset direction
        var result = new List<Vec2>(solution[0].Count);
        foreach (var pt in solution[0])
        {
            result.Add(new Vec2(pt.X / Scale, pt.Y / Scale));
        }

        // For a simple line offset, extract the relevant half of the buffer polygon
        return ExtractOffsetSide(linePoints, result, offsetDistance);
    }

    /// <summary>
    /// Extract one side from a buffer polygon around a line
    /// </summary>
    private List<Vec2>? ExtractOffsetSide(List<Vec2> originalLine, List<Vec2> bufferPolygon, double offsetDistance)
    {
        if (bufferPolygon.Count < 4)
            return bufferPolygon;

        // Find the points in the buffer that are on the offset side
        // by checking which points are approximately offsetDistance away from the original line
        var result = new List<Vec2>();
        double targetDist = System.Math.Abs(offsetDistance);
        double tolerance = targetDist * 0.5; // Allow some variation

        foreach (var bufferPt in bufferPolygon)
        {
            // Find minimum distance to original line
            double minDist = double.MaxValue;
            for (int i = 0; i < originalLine.Count - 1; i++)
            {
                double dist = PointToSegmentDistance(bufferPt, originalLine[i], originalLine[i + 1]);
                minDist = System.Math.Min(minDist, dist);
            }

            // Keep points that are roughly the right distance away
            if (System.Math.Abs(minDist - targetDist) < tolerance)
            {
                result.Add(bufferPt);
            }
        }

        // If we got enough points, sort them along the original line direction
        if (result.Count >= 2)
        {
            // Sort by projection onto original line direction
            var lineDir = new Vec2(
                originalLine[originalLine.Count - 1].Easting - originalLine[0].Easting,
                originalLine[originalLine.Count - 1].Northing - originalLine[0].Northing);
            double len = System.Math.Sqrt(lineDir.Easting * lineDir.Easting + lineDir.Northing * lineDir.Northing);
            if (len > 0)
            {
                lineDir = new Vec2(lineDir.Easting / len, lineDir.Northing / len);
                result.Sort((a, b) =>
                {
                    double projA = a.Easting * lineDir.Easting + a.Northing * lineDir.Northing;
                    double projB = b.Easting * lineDir.Easting + b.Northing * lineDir.Northing;
                    return projA.CompareTo(projB);
                });
            }
            return result;
        }

        // Fallback: just return first half of buffer (rough approximation)
        return bufferPolygon.Take(bufferPolygon.Count / 2).ToList();
    }

    /// <summary>
    /// Calculate distance from a point to a line segment
    /// </summary>
    private double PointToSegmentDistance(Vec2 point, Vec2 segA, Vec2 segB)
    {
        double dx = segB.Easting - segA.Easting;
        double dy = segB.Northing - segA.Northing;
        double lenSq = dx * dx + dy * dy;

        if (lenSq < 1e-10)
            return System.Math.Sqrt(
                (point.Easting - segA.Easting) * (point.Easting - segA.Easting) +
                (point.Northing - segA.Northing) * (point.Northing - segA.Northing));

        double t = System.Math.Max(0, System.Math.Min(1,
            ((point.Easting - segA.Easting) * dx + (point.Northing - segA.Northing) * dy) / lenSq));

        double projX = segA.Easting + t * dx;
        double projY = segA.Northing + t * dy;

        return System.Math.Sqrt(
            (point.Easting - projX) * (point.Easting - projX) +
            (point.Northing - projY) * (point.Northing - projY));
    }

    /// <summary>
    /// Calculate headings for each point in the polygon based on adjacent points.
    /// </summary>
    /// <param name="points">Polygon points</param>
    /// <returns>Points with headings as Vec3 (X, Y, Heading in radians)</returns>
    public List<Vec3> CalculatePointHeadings(List<Vec2> points)
    {
        var result = new List<Vec3>(points.Count);

        if (points == null || points.Count < 3)
            return result;

        for (int i = 0; i < points.Count; i++)
        {
            var prev = points[(i - 1 + points.Count) % points.Count];
            var next = points[(i + 1) % points.Count];

            // Calculate heading from direction vector between prev and next
            double dx = next.Easting - prev.Easting;
            double dy = next.Northing - prev.Northing;
            double heading = System.Math.Atan2(dx, dy);

            result.Add(new Vec3(points[i].Easting, points[i].Northing, heading));
        }

        return result;
    }
}

/// <summary>
/// Corner join types for polygon offset
/// </summary>
public enum OffsetJoinType
{
    /// <summary>Round corners (smooth)</summary>
    Round,
    /// <summary>Miter corners (sharp, extended)</summary>
    Miter,
    /// <summary>Square corners (flat)</summary>
    Square
}
