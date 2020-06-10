using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WirelessDisplay.Common
{
    public interface IRemoteScriptRunner
    {
        /// <summary>
        /// Asks the remote computer to execute a script, and returns the script's 
        /// output.
        /// </summary>
        /// <param name="scriptName"> 
        /// The name of the script without the platform-dependent 
        /// File-Extension (.sh or .bat). 
        /// </param>
        /// <param name="scriptArgs"> 
        /// A whitespace-separated list of arguments passed to 
        /// the script. 
        /// </param>
        /// <param name="stdin"> 
        /// A string passed as stdin to the script. 
        /// </param>
        /// <returns> 
        /// First item of the Tuple: The exit-code of the script.
        /// Second item of Tuple: The lines, the script sent to its stdout.
        /// Third item of Tuple: The lines, the script sent to its stderr.
        /// </returns>
        Task<Tuple<int,List<string>,List<string>>> RunAndWaitForScript( 
                            string scriptName, string scriptArgs, 
                            string stdin="", int timeoutMillis=10000 );


        /// <summary>
        /// Asks the remote computer to start a script, which (normally) 
        /// continues to run in background.
        /// </summary>
        /// <param name="scriptName"> 
        /// The name of the script without the platform-dependent 
        /// File-Extension (.sh or .bat). 
        /// </param>
        /// <param name="scriptArgs"> 
        /// A whitespace-separated list of arguments passed to the script. 
        /// </param>
        /// <param name="stdin"> 
        /// A string passed as stdin to the script.
        /// </param>
        /// <param name="shortTimeoutMillis">
        /// After starting the process, this time in milliseconds is waited. After 
        /// this time it is verified, that the process is still running.
        /// </param>
        /// <returns> 
        /// The process-ID of the started shell. This can later be used to
        /// stop (kill) the process.
        /// </returns>
        Task<int> StartScript(string scriptName, string scriptArgs, 
                            string stdin="", int shortTimeoutMillis=0);


        /// <summary> 
        /// Asks the remote computer to stop (kill) a previously started script. 
        /// </summary>
        /// <param name="processId"> 
        /// The process-ID of the previously started remote script. (The 
        /// return-value from StartScript()).
        /// </param>
        /// <returns> 
        /// First item of the Tuple: The exit-code of the script.
        /// Second item of Tuple: The lines, the script sent to its stdout.
        /// Third item of Tuple: The lines, the script sent to its stderr.
        /// </returns>
        Task<Tuple<int,List<string>,List<string>>> StopScript( int processId );


        /// <summary> 
        /// Test, if a previously started script on the remote computer is still running.
        /// </summary>
        /// <param name="processId"> 
        /// The process-ID of the previously started remote script. (The 
        /// return-value from StartScript()).
        /// </param>
        /// <returns> 
        /// True, if the script is still running, false otherwise. 
        /// </retuns>
        Task<bool> IsScriptRunning(int processId);
    }
}