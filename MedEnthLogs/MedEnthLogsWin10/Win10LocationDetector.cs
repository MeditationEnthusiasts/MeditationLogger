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
        public double Latitude { get; }

        /// <summary>
        /// Longitude Position.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Accuracy of location in METERS.
        /// </summary>
        public double Accuracy { get; }

        // -------- Functions --------

        /// <summary>
        /// Refreshes the position information.
        /// </summary>
        public void RefreshPosition()
        {

        }
    }
}
