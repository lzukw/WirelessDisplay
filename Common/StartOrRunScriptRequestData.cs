

namespace WirelessDisplay.Common
{
    /// <summary>
    /// Objects of this type are sent from the REST-API-client to the REST-API-Server
    /// when making a POST-request to run or start a script on the server.
    /// </summary>
    public class StartOrRunScriptRequestData
    {
        /// <summary>
        /// The name of the script to run on REST-API-Server. This name must
        /// not contain the file-extension.
        /// </summary>
        public string ScriptName { get; set; }

        /// <summary>
        /// A whitespace separated list of command-line-arguments to pass to
        /// the script.
        /// </summary>
        public string ScriptArgs { get; set; }

        /// <summary>
        /// This string is passed as stdin-stream to the script.
        /// </summary>
        public string StdIn { get; set; }
    }

}