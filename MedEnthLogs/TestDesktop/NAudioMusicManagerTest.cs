// 
// Meditation Logger.
// Copyright (C) 2015-2016  Seth Hendrick.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedEnthDesktop;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class NAudioMusicManagerTest
    {
        // -------- Fields --------

        /// <summary>
        /// Unit under test.
        /// </summary>
        NAudioMusicManager uut;

        /// <summary>
        /// Public domain mp3 from http://publicsounds.org/
        /// </summary>
        const string mp3FileLocation = @"..\..\..\TestCore\TestFiles\thunder.mp3";

        /// <summary>
        /// Public domain wav (converted from mp3) from http://publicsounds.org/
        /// </summary>
        const string wavFileLocation = @"..\..\..\TestCore\TestFiles\thunder.wav";


        // -------- Setup/Teardown --------

        [SetUp]
        public void TestSetup()
        {
            this.uut = new NAudioMusicManager();
        }

        // -------- Test Functions --------

        [Test]
        public void ValidateSuccessTest()
        {
            Assert.DoesNotThrow(
                delegate ()
                {
                    this.uut.Validate( mp3FileLocation );
                }
            );

            Assert.DoesNotThrow(
                delegate ()
                {
                    this.uut.Validate( wavFileLocation );
                }
            );
        }

        /// <summary>
        /// Ensures passing in garbage data results in an exception.
        /// </summary>
        [Test]
        public void ValidateFailureTest()
        {
            // Null will fail.
            Assert.Throws<ArgumentException>(
                delegate ()
                {
                    this.uut.Validate( null );
                }
            );

            // Empty string will fail.
            Assert.Throws<ArgumentException>(
                delegate ()
                {
                    this.uut.Validate( "" );
                }
            );

            // Fail for file that doesn't exist.
            Assert.Throws<FileNotFoundException>(
                delegate ()
                {
                    this.uut.Validate( "derp.mp3" );
                }
            );

            // Fail for file that exists but is not supported.
            Assert.Throws<PlatformNotSupportedException>(
                delegate ()
                {
                    this.uut.Validate( @"..\..\..\TestCore\TestFiles\MissingLat.xml" );
                }
            );
        }

        [Test]
        public void GetLengthOfAudioFileTest()
        {
            TimeSpan mp3TimeSpan = uut.GetLengthOfFile( mp3FileLocation );

            Assert.AreEqual(
                new TimeSpan( 0, 0, 20 ),

                // We don't care about milliseconds.
                new TimeSpan( mp3TimeSpan.Hours, mp3TimeSpan.Minutes, mp3TimeSpan.Seconds )
            );


            TimeSpan wavTimeSpan = uut.GetLengthOfFile( mp3FileLocation );
            Assert.AreEqual(
                new TimeSpan( 0, 0, 20 ),

                // We don't care about milliseconds.
                new TimeSpan( wavTimeSpan.Hours, wavTimeSpan.Minutes, wavTimeSpan.Seconds )
            );
        }
    }
}
