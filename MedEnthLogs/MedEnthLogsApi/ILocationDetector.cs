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
        double Latitude { get; }

        /// <summary>
        /// Longitude Position.
        /// </summary>
        double Longitude { get; }

        /// <summary>
        /// Accuracy of location in METERS.
        /// </summary>
        double Accuracy { get; }

        /// <summary>
        /// Refreshes the position information.
        /// </summary>
        void RefreshPosition();
    }
}
