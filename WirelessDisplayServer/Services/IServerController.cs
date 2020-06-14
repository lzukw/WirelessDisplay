using System;
using System.Diagnostics;

namespace WirelessDisplayServer.Services
{
    /// <summary>
    /// Object implementing this interface can start and stop the 
    /// sripting-REST-API-Serverm and provide an event, to get the lines 
    /// written by the server to its stdout and stderr.
    /// </summary>
    public interface IServerController
    {
        /// <summary> Starts the Scripting-REST-API-Server. </summary>
        /// <param name="PortNo"> 
        /// The port-number the Scripting-REST-API-Server listens on
        /// </param>
        void StartServerInBackground(UInt16 PortNo);
        
        /// <summary> Stops the Scripting-REST-API-Server. </summary>
        void StopServer();

        /// <summary>
        /// Raised, when the server wrote a line to its stdout or stderr.
        /// </summary>
        event DataReceivedEventHandler ServerOutputReceived;
    }

}