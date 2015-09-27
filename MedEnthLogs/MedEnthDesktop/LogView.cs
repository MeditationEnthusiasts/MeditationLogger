using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
