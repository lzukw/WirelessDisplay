using System.Text.Json.Serialization;

namespace WirelessDisplay.Common.JsonObjects
{
    public class QueryScriptResponseData
    {
        /// <summary> True, if the script-stae could successfully be retrieved </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set;}

        /// <summary> 
        /// If the REST-API-server cannot query the script-state, this
        /// string contains an error-message.
        /// </summary>
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// True, if the shell-process executing the script is still running.
        /// </summary>
        /// <value></value>
        [JsonPropertyName("isRunning")]
        public bool IsRunning { get; set; }
    }

}