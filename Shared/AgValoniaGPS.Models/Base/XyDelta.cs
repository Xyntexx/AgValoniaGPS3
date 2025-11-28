using System;

namespace AgValoniaGPS.Models
{
    public struct XyDelta
    {
        public XyDelta(XyCoord fromCoord, XyCoord toCoord)
        {
            DeltaX = toCoord.X - fromCoord.X;
            DeltaY = toCoord.Y - fromCoord.Y;
        }

        public XyDelta(double deltaX, double deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }

        public double DeltaX { get; }
        public double DeltaY { get; }

    }
}
