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

using System.Windows.Forms;
using MedEnthLogsApi;
using System.Globalization;

namespace MedEnthDesktop
{
    public partial class LogView : UserControl
    {
        public LogView( ILog log )
        {
            InitializeComponent();
            this.DurationValueLabel.Text = log.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture ) + " minutes";
            this.TechniqueValueLabel.Text = log.Technique;
            this.CommentValueTextBox.Text = log.Comments;
            this.StartDateLabel.Text = log.StartTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" );
        }
    }
}
