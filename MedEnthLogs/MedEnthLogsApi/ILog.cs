using System;

namespace MedEnthLogsApi
{
    /// <summary>
    /// Interface to a log object.
    /// </summary>
    public interface ILog
    {
        int Id { get; }

        /// <summary>
        /// When the session starts
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// When the session ends
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// When the session was first recorded
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// The last time this log was edited.
        /// (UTC, the UI must convert it to local time).
        /// </summary>
        DateTime EditTime { get; }

        /// <summary>
        /// How long the session lasted.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The Technique Used.
        /// specifies.
        /// </summary>
        string Technique { get; }

        /// <summary>
        /// The comments the user wrote about the session.
        /// </summary>
        string Comments { get; }

        /// <summary>
        /// The latitude of where the session took place.
        /// null if no location specified.
        /// </summary>
        decimal? Latitude { get; set; }

        /// <summary>
        /// The longitude of where the session took place.
        /// null if no location specified.
        /// </summary>
        decimal? Longitude { get; set; }

        /// <summary>
        /// Ensures the log is in a good state.  Should be called before saving it.
        /// Throws Exceptions if not.
        /// </summary>
        void Validate();
    }
}
