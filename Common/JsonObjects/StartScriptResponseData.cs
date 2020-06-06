

namespace WirelessDisplay.Common.JsonObjects
{
    /// <summary>
    /// Objects of this type are sent to the REST-API-client by the REST-API-Server
    /// in response to a POST-request to start a script on the server.
    /// </summary>
    public class StartScriptResponseData
    {
        /// <summary> True, if the script could be started successfully. </summary>
        public bool Success { get; set;}

        /// <summary> 
        /// If the REST-API-server cannot start the script, this
        /// string contains an error-message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The process-ID of the shell that is running the script. This
        /// number was returned by the server, when the script was started.
        /// </summary>
        public int ProcessId { get; set; }
    }
}