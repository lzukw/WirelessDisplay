using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WirelessDisplay.Common.JsonObjects
{
    /// <summary>
    /// Objects of this type are sent to the REST-API-client by the REST-API-server
    /// after receiving a POST-request to to run or stop a script on the server.
    /// </summary>
    public class RunOrStopScriptResponseData
    {
        /// <summary> True, if the Script could successfully be run / stopped </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set;}

        /// <summary> 
        /// If the REST-API-server cannot run or stop the script, this
        /// string contains an error-message.
        /// </summary>
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        /// <summary> The exit-code from the script. </summary>
        [JsonPropertyName("scriptExitCode")]
        public int ScriptExitCode { get; set; }        
        
        /// <summary> The lines written by the script to its stdout. </summary>
        [JsonPropertyName("stdoutLines")]
        public List<string> StdoutLines { get; set; }

        /// <summary> The lines written by the script to its stderr. </summary>
        [JsonPropertyName("stderrLines")]
        public List<string> StderrLines { get; set; }
    }

}