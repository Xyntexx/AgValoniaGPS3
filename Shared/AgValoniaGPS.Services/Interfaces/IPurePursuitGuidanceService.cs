using AgValoniaGPS.Models.Guidance;

namespace AgValoniaGPS.Services.Interfaces
{
    /// <summary>
    /// Service for Pure Pursuit guidance algorithm calculations.
    /// </summary>
    public interface IPurePursuitGuidanceService
    {
        /// <summary>
        /// Calculate steering guidance using Pure Pursuit algorithm for AB line.
        /// </summary>
        /// <param name="input">Pure Pursuit algorithm input parameters</param>
        /// <returns>Pure Pursuit guidance output</returns>
        PurePursuitGuidanceOutput CalculateGuidanceABLine(PurePursuitGuidanceInput input);
    }
}
