using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedEnthLogsApi;

namespace MedEnthLogsWin10
{
    public class Win10LocationDetector : ILocationDetector
    {
        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public Win10LocationDetector()
        {

        }

        // -------- Properties --------

        /// <summary>
        /// Latitude Position.
        /// </summary>
        public decimal Latitude { get; }

        /// <summary>
        /// Longitude Position.
        /// </summary>
        public decimal Longitude { get; }

        /// <summary>
        /// True if we have a valid position, else false.
        /// </summary>
        public bool IsReady { get; private set; }

        // -------- Functions --------

        /// <summary>
        /// Refreshes the position information.
        /// </summary>
        /// <returns>True if we got a position.</returns>
        public bool RefreshPosition()
        {
            return false;
        }
    }
}
