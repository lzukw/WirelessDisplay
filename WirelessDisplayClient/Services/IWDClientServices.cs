using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WirelessDisplayClient.Services
{
    /// <summary>
    /// Opbejct implementing this interface can perform all the work needed by the
    /// WirelessDisplayClient's MainWindowViewModel.
    /// </summary>
    public interface IWDClientServices
    {
        /// <summary>
        /// Sets the IP-Address and port of the remote scripting-REST-API-server.
        /// And afterwards gets the inital screen-resolution of the remote computer.static
        /// This method must be called, before any other scripts can be executed
        /// on the remote server.
        /// </summary>
        /// <param name="ipAddress">
        /// The IP-Address of the scripting-REST-API-server.
        /// </param>
        /// <param name="port">
        /// The port-number of the scripting-REST-API-server. 
        /// </param>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// Connection was not successfull (for example because of wrong IP-Address, or
        /// the remote server is not running.)
        /// </exception>
        Task ConnectToServer(string ipAddress, UInt16 port);

        /// <summary> Returns the initial screen-resolution of the local computer. </summary>
        /// <returns>A string like "1024x768".</returns>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The query of the local screen-resolution failed.
        /// </exception>
        string GetInitalLocalScreenResolution();
        
        /// <summary> Returns the current screen-resolution of the local computer. </summary>
        /// <returns>A string like "1024x768".</returns>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The query of the local screen-resolution failed.
        /// </exception>
        string GetCurrentLocalScreenResolution();
        
        /// <summary> 
        /// Returns all available screen-resolutions of the local computer. 
        /// </summary>
        /// <returns>A list of strings with the screen-resolutions.</returns>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The query of the local screen-resolutions failed.
        /// </exception>
        List<string> GetAvailableLocalScreenResolutions();

        /// <summary>
        /// Changes the screen-resolution of the local computer
        /// </summary>
        /// <param name="screenResolution">
        /// One of the strings listed by GetAvailableLocalScreenResolutions().
        /// For example a string like "1024x768".
        /// </param>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The local screen-resolution could not be set.
        /// </exception>
        void SetLocalScreenResolution(string screenResolution);

        /// <summary> Returns the initial screen-resolution of the remote computer. </summary>
        /// <returns>A string like "1024x768".</returns>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The query of the remote screen-resolution failed.
        /// </exception>
        Task<string> GetInitalRemoteScreenResolution();

        /// <summary> Returns the current screen-resolution of the remote computer. </summary>
        /// <returns>A string like "1024x768".</returns>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The query of the remote screen-resolution failed.
        /// </exception>
        Task<string> GetCurrentRemoteScreenResolution();

        /// <summary> 
        /// Returns all available screen-resolutions of the remote computer. 
        /// </summary>
        /// <returns>A list of strings with the screen-resolutions.</returns>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The query of the remote screen-resolutions failed.
        /// </exception>
        Task<List<string>> GetAvailableRemoteScreenResolutions();

        /// <summary>
        /// Changes the screen-resolution of the remote computer
        /// </summary>
        /// <param name="screenResolution">
        /// One of the strings listed by GetAvailableRemoteScreenResolutions().
        /// For example a string like "1024x768".
        /// </param>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The remote screen-resolution could not be changed.
        /// </exception>
        Task SetRemoteScreenResolution(string screenResolution);

        /// <summary>
        /// Start streaming-source on the local computer by starting a local script.
        /// </summary>
        /// <param name="streamType">
        /// One of the stream-types given in enum StreamType (VNC or FFmpeg).
        /// </param>
        /// <param name="sinkIpAddress">
        /// The IP-Address of the remote 'projecting'-computer to send the stream to.
        /// </param>
        /// <param name="portNo">
        /// The port-Number used for the remote streaming-sink to listen on.
        /// </param>
        /// <param name="streamScreenResolution">
        /// A string contating the screen-resolution used for streaming.
        /// </param>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The local streaming source could not be started, for example because of
        /// an error in the script starting the streaming-source.
        /// </exception>
        void StartLocalStreamingSource( string streamType,
                                     string sinkIpAddress,
                                     UInt16 port,
                                     string streamScreenResolution );


        /// <summary>
        /// Stops streaming-source on the local computer.
        /// </summary>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// If the process could not be killed (should never occur)
        /// </exception>
        void StopLocalStreamingSource();

        /// <summary>
        /// Starts the streaming-sink on the remote computer.
        /// </summary>
        /// <param name="streamType">
        /// One of the stream-types given in enum StreamType (VNC or FFmpeg).
        /// </param>
        /// <param name="port">
        /// The port-Number used for the remote streaming-sink to listen on.
        /// </param>
        /// <param name="streamScreenResolution">
        /// A string contating the screen-resolution used for streaming.
        /// </param>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The remote streaming-sink could not be started.
        /// </exception>
        Task StartRemoteStreamingSink( string streamType, 
                                       UInt16 port,
                                       string streamScreenResolution );

        /// <summary>
        /// Stops the streaming-sink on the remote computer.
        /// </summary>
        /// <expection cref="WirelessDisplay.Common.WDException">
        /// The remote streaming-sink could not be stopped.
        /// </exception>
        Task StopRemoteStreamingSink();

        /// <summary>
        /// Starts the remote script, that prevents the display from blanking.
        /// </summary>
        /// <param name="seconds">
        /// The maximum time (in seconds), the program should run. If this time
        /// ellapses, the prevent-display-blanking-script shuts down alone.
        /// </param>
        Task StartRemotePreventDisplayBlanking( int seconds = 7200);

        /// <summary>
        /// Stops the remote script, that prevents the display from blanking,
        /// if this script is still running
        /// </summary>
        Task StopRemotePreventDisplayBlanking();
    }
}