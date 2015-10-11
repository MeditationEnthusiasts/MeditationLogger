namespace MedEnthLogsApi
{
    /// <summary>
    /// Interface for finding the location of the session.
    /// Interface since how Win32 does this is different from Win10.
    /// </summary>
    public interface ILocationDetector
    {
        /// <summary>
        /// Latitude Position.
        /// </summary>
        decimal Latitude { get; }

        /// <summary>
        /// Longitude Position.
        /// </summary>
        decimal Longitude { get; }

        /// <summary>
        /// True if we have a valid position, else false.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Refreshes the position information.
        /// </summary>
        /// <returns>True if we got a position.</returns>
        bool RefreshPosition();
    }
}
