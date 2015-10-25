// 
// Meditation Logger.
// Copyright (C) 2015  Seth Hendrick.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MedEnthLogsApi
{
    public class JsonExporter
    {
        public static void ExportJson( Stream outFile, LogBook logBook )
        {
            using ( StreamWriter writer = new StreamWriter( outFile ) )
            {
                foreach ( Log log in logBook.Logs )
                {
                    JObject o = new JObject();
                    o["CreationTime"] = log.CreateTime.ToString( "o" );
                    o["EditTime"] = log.EditTime.ToString( "o" );
                    o["StartTime"] = log.StartTime.ToString( "o" );
                    o["EndTime"] = log.EndTime.ToString( "o" );
                    o["Technique"] = log.Technique;
                    o["Comments"] = log.Comments;
                    o["Latitude"] = log.Latitude.HasValue ? log.Latitude.ToString() : string.Empty;
                    o["Longitude"] = log.Longitude.HasValue ? log.Longitude.ToString() : string.Empty;

                    writer.WriteLine( o.ToString() );
                }
            }
        }
    }
}
