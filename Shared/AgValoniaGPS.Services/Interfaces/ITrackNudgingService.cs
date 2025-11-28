using AgValoniaGPS.Models.Base;
using AgValoniaGPS.Models.Track;

namespace AgValoniaGPS.Services.Interfaces
{
    /// <summary>
    /// Service for track nudging geometric calculations.
    /// </summary>
    public interface ITrackNudgingService
    {
        /// <summary>
        /// Nudge an AB line by perpendicular distance.
        /// </summary>
        /// <param name="input">AB line nudge input parameters</param>
        /// <returns>New AB line points</returns>
        ABLineNudgeOutput NudgeABLine(ABLineNudgeInput input);

        /// <summary>
        /// Nudge a curve by perpendicular distance with filtering and smoothing.
        /// </summary>
        /// <param name="input">Curve nudge input parameters</param>
        /// <returns>New curve points</returns>
        CurveNudgeOutput NudgeCurve(CurveNudgeInput input);
    }
}
