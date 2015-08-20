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
        /// Where the user had the session.
        /// This can either be GPS coordinates or a location the user
        /// specifies.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// The comments the user wrote about the session.
        /// </summary>
        string Comments { get; }
    }
}
