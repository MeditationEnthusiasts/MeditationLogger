//
// Meditation Logger.
// Copyright (C) 2015-2017  Seth Hendrick.
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

namespace MeditationEnthusiasts.MeditationLogger.Api
{
    public static class ErrorCodes
    {
        /// <summary>
        /// No Errors Happened.
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// Error from the API.
        /// </summary>
        public const int ApiError = 1;

        /// <summary>
        /// User error.  Usually from entering a bad parameter.
        /// </summary>
        public const int UserError = 2;

        /// <summary>
        /// Error from the server other than Admin errors.
        /// </summary>
        public const int HttpError = 3;

        /// <summary>
        /// Error when running the server.
        /// Admin or a netsh is needed to launch the server.
        /// </summary>
        public const int AdminNeeded = 9;
    }
}