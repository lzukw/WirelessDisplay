using System.Text.Json.Serialization;

namespace WirelessDisplay.Common.JsonObjects
{
    /// <summary>
    /// Objects of this type are sent from the REST-API-client to the 
    /// REST-API-Server when making a POST-request to stop a script on the server.
    /// </summary>
    public class QueryOrStopScriptRequestData
    {
        /// <summary>
        /// The process-ID of the shell that is running the script. This
        /// number was returned by the server, when the script was started.
        /// </summary>
        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }
    }
}
