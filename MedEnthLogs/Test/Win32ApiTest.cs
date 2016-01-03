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
                new Win32LocationDetector(),
                this.mockTimer,
                this.mockAudio,
                new SQLite.Net.Platform.Win32.SQLitePlatformWin32()
            );
        }

        private const string projectDir = @"..\..\";
    }
}

