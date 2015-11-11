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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedEnthDesktop
{
    public partial class MeditateView : UserControl
    {
        /// <summary>
        /// Used for random number generation.
        /// </summary>
        Random rng = new Random();

        /// <summary>
        /// Constructor
        /// </summary>
        public MeditateView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Updates the background color randomly.
        /// </summary>
        public void UpdateBackground()
        {
            DateTime time = DateTime.Now;
            this.BackColor = Color.FromArgb(
                rng.Next( 175, 240 ),
                rng.Next( 175, 240 ),
                rng.Next( 175, 240 )
            );
        }
    }
}
