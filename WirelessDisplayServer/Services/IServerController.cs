using System;
using System.Diagnostics;

namespace WirelessDisplayServer.Services
{
    public interface IServerController
    {
        void StartServerInBackground(UInt16 PortNo);
        void StopServer();

        /// <summary>
        /// Raised, when the server wrote a line to its stdout or stderr.
        /// </summary>
        event DataReceivedEventHandler ServerOutputReceived;
    }

}