

namespace WirelessDisplay.Common.JsonObjects
{
    public class QueryScriptResponseData
    {
        /// <summary> True, if the script-stae could successfully be retrieved </summary>
        public bool Success { get; set;}

        /// <summary> 
        /// If the REST-API-server cannot query the script-state, this
        /// string contains an error-message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// True, if the shell-process executing the script is still running.
        /// </summary>
        /// <value></value>
        public bool IsRunning { get; set; }
    }

}