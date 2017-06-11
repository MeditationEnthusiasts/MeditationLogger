using System;
using MedEnthLogsApi;
using MedEnthLogsDesktop;

namespace TestCommon
{
    public partial class LogsApiTest
    {
        public Api GetApi()
        {
            return new Api(
                new LinuxLocationDetector(),
                this.mockTimer,
                this.mockAudio,
                new SQLite.Net.Platform.Generic.SQLitePlatformGeneric()
            );
        }

        private const string projectDir = "../../../";
    }
}

