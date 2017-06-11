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

enum State {
    Idle, // Nothing is happending, the user didn't press "Start" yet
    Meditating, // User is currently meditating.
    Complete, // User completed meditation and is prompted to fill in additional information.
    Download, // A download button appears to download the XML file.
}

/// This is a static web page that can be hosted any where.
/// It will generate an XML document that can be used to import into
/// Meditation Logger so anyone can record meditations anywhere!
class MeditationLogger {
    mainElement: HTMLElement;
    span: HTMLElement;
    timerToken: number;
    currentState: State;
    stateButton: HTMLButtonElement;

    constructor(element: HTMLElement) {
        this.mainElement = element;
        this.mainElement.innerHTML += "The time is: ";
        this.span = document.createElement('span');
        this.mainElement.appendChild(this.span);
        this.span.innerText = new Date().toUTCString();

        this.stateButton = document.getElementById("changeStateButton") as HTMLButtonElement;
        this.stateButton.disabled = true;
    }

    start() {
        this.timerToken = setInterval(() => this.span.innerHTML = new Date().toUTCString(), 500);
    }

    stop() {
        clearTimeout(this.timerToken);
    }
}

window.onload = () => {
    var el = document.getElementById('entry-content');
    var greeter = new MeditationLogger(el);
    greeter.start();
};