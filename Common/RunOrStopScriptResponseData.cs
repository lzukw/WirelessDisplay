using System.Collections.Generic;

namespace WirelessDisplay.Common
{
    /// <summary>
    /// Objects of this type are sent to the REST-API-client by the REST-API-server
    /// after receiving a POST-request to to run or stop a script on the server.
    /// </summary>
    public class RunOrStopScriptResponseData
    {
        /// <summary> True, if the Script could successfully be run / stopped </summary>
        public bool Success { get; set;}

        /// <summary> 
        /// If the REST-API-server cannot run or stop the script, this
        /// string contains an error-message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary> The exit-code from the script. </summary>
        public int ScriptExitCode { get; set; }        
        
        /// <summary> The lines written by the script to its stdout. </summary>
        public List<string> StdoutLines { get; set; }

        /// <summary> The lines written by the script to its stderr. </summary>
        public List<string> StderrLines { get; set; }
    }

}