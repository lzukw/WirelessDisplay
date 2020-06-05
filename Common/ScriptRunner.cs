using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WirelessDisplay.Common
{
    /// <summary>
    ///     A class for starting and stopping scripts (shell-scripts / batch-files).
    ///     The shell of the operating-system (bash, cmd.exe) is used to run the
    ///     scripts.
    /// </summary>
    public class ScriptRunner : IScriptRunner
    {
        //#####################################################################
        //## Magic strings, private fields and constructor
        //#####################################################################
        #region 

        private const string PLACEHOLDER_SCRIPT_PATH="{SCRIPT}";
        private const string PLACEHOLDER_SCRIPT_ARGS="{SCRIPT_ARGS}";

        private readonly ILogger<ScriptRunner> _logger;

        /// <summary> The shell used to run the scripts (bash, cmd, powershell) </summary>
        private readonly string _shell;

        /// <summary> 
        /// The commandline-arguments passed to the shell, in order to execute the script.
        /// Must contain {SCRIPT} and {SCRIPTARGS}.
        /// </summary>
        private readonly string _shellArgsTemplate;

        /// <summary> The directory, where the scripts reside </summary>
        private readonly DirectoryInfo _scriptDirectory;
        
        /// <summary> The file-extension for scripts (for example .sh, .bat). </summary>
        private readonly string _scriptExtension;

        /// <summary>
        /// This dictionary contains all started processes. The key is the process-ID
        /// of the started shell, which is executing the script.
        /// </summary>
        private Dictionary<int,Process> _processes = new Dictionary<int, Process>();


        /// <summary> Constructor. </summary>
        /// <param name="logger"> Used for Logging. </param>
        /// <param name="shell"> 
        /// A string containing the name of the executable shell (bash, cmd, powershell)
        /// </param>
        /// <param name="shellArgsTemplate">
        /// This string must contain the two magics {SCRIPT} and {SCRIPT_ARGS}. After the
        /// magigs have benn replaces shellArgsTemplate is passed as command-line
        /// argument(s) to the shell. For example: On Linux a script "setScreenres.sh"
        /// taking two arguments (width and heigt)
        /// can be executed by <code>bash -c "setScreenres.sh 1024 768"</code>. In this
        /// case shell should be "bash" and shellArgsTemplate should be 
        /// "-c {SCRIPT} {SCRIPT_ARGS}".
        /// </param>
        /// <param name="scriptDirectory">
        /// The path to a directory, that contains the scripts to be executed.
        /// </param>
        /// <param name="scriptExtension">
        /// This file-extension will be appended to the parameter 
        /// <code>scriptName</code> in the methods for running/starting a script. 
        /// For example "sh". "bat" or "ps1".
        /// So the full path of a script is scriptDirectory/scritName.scriptExtension .
        /// </param>
        public ScriptRunner(ILogger<ScriptRunner> logger, 
                            string shell, 
                            string shellArgsTemplate, 
                            DirectoryInfo scriptDirectory,
                            string scriptExtension)
        {
            _logger = logger;
            _shell = shell;
            _shellArgsTemplate = shellArgsTemplate;
            _scriptDirectory = scriptDirectory;
            if (! _scriptDirectory.Exists )
            {
                _logger?.LogCritical($"FATAL: The script-directory {_scriptDirectory.FullName} doesn't exist.");
                throw new WDFatalException($"FATAL: The script-directory {_scriptDirectory.FullName} doesn't exist.");
            }
            _scriptExtension = scriptExtension;
        }

        #endregion

        //#####################################################################
        //## Implementing the API-methds (offered through the interface)
        //#####################################################################
        #region

        /// <see cref="" >IScriptRunner.RunAndWaitForScript()</see>
	    Tuple<int,List<string>,List<string>> IScriptRunner.RunAndWaitForScript(
                        string scriptName, string scriptArgs, string stdin, 
                        int timeoutMillis )
        {           
            ProcessStartInfo startInfo = preapareProcessStartInfo(scriptName, scriptArgs);

            var stdoutLines = new List<string>();
            var stderrLines = new List<string>();
            int exitCode;

            using (var shortProcess = new Process { StartInfo = startInfo } )
            {

                try
                {
                    shortProcess.Start();

                    // null is allowed (nothing is written)
                    shortProcess.StandardInput.Write(stdin);
                }
                catch (Exception e)
                {
                    string msg = $"Could not start process '{shortProcess.StartInfo.FileName} {shortProcess.StartInfo.Arguments}': {e.Message}";
                    _logger?.LogError(msg);
                    throw new WDException(msg);
                }

                                //Wait until screenres-process exits.
                bool exited = shortProcess.WaitForExit(timeoutMillis);

                if (! exited )
                {
                    string msg = $"Process not finished within {timeoutMillis} Milliseconds: '{shortProcess.StartInfo.FileName} {shortProcess.StartInfo.Arguments}'. Scripting Error?";
                    _logger?.LogError(msg);
                    throw new WDException(msg);
                }               

                exitCode = shortProcess.ExitCode;

                string line;
                while ((line = shortProcess.StandardOutput.ReadLine()) != null)
                {
                    stdoutLines.Add(line);
                }

                while ((line = shortProcess.StandardError.ReadLine()) != null)
                {
                    stderrLines.Add(line);
                }

                _logger?.LogInformation($"Successfully ran script '{scriptName}{_scriptExtension} {scriptArgs}'");

                return Tuple.Create(exitCode, stdoutLines, stderrLines);
            }
        }


        /// <see cref="" >IScriptRunner.StartScript()</see>
        int IScriptRunner.StartScript(string scriptName, string scriptArgs, 
                                        string stdin, int shortTimeoutMillis)        
        {
            ProcessStartInfo startInfo = preapareProcessStartInfo(scriptName, scriptArgs);

            var longProcess = new Process { StartInfo = startInfo };
            
            try
            {
                longProcess.Start();

                // null is allowed (nothing is written)
                longProcess.StandardInput.Write(stdin);
            }
            catch (Exception e)
            {
                string msg = $"Could not start process '{longProcess.StartInfo.FileName} {longProcess.StartInfo.Arguments}': {e.Message}";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

                            //Wait until screenres-process exits.
            bool exited = longProcess.WaitForExit(shortTimeoutMillis);

            if ( exited )
            {
                string msg = $"Process has exited immidiately: '{longProcess.StartInfo.FileName} {longProcess.StartInfo.Arguments}'. Scripting Error?";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }               

            // save the process for later being able to kill it.
            int processId = longProcess.Id;
            _processes[processId] = longProcess;
            
            _logger?.LogInformation($"Successfully started script '{scriptName}{_scriptExtension} {scriptArgs}. Process-ID is {processId}.");
            return processId;
        }


        /// <see cref="" >IScriptRunner.StopScript()</see>
        Tuple<int,List<string>,List<string>> IScriptRunner.StopScript(int processId)
        {
            Process processToStop = _processes.GetValueOrDefault(processId);

            if (processToStop == default(Process) )
            {
                string msg = $"Tried to stop process with process-ID {processId}, but no such process has been started.";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

            var stdoutLines = new List<string>();
            var stderrLines = new List<string>();
            int exitCode;

            // if the process already has terminated, then this is supposed to be o.k.
            if ( ! processToStop.HasExited )
            {
                // here, no exception should ever occur (otherwise it is a bug).
                processToStop.Kill( entireProcessTree : true ); 
            }              
            
            exitCode = processToStop.ExitCode;

            string line;
            while ( (line = processToStop.StandardOutput.ReadLine()) != null )
            {
                stdoutLines.Add(line);
            }

            while ( (line = processToStop.StandardError.ReadLine()) != null )
            {
                stderrLines.Add(line);
            }

            _logger?.LogInformation($"Successfully stopped script-process '{processToStop.StartInfo.FileName} {processToStop.StartInfo.Arguments}' with process-ID {processId}");

            _processes.Remove(processId); 
            processToStop.Dispose();
            processToStop = null;

            return Tuple.Create(exitCode, stdoutLines, stderrLines);           
        }


        /// <see cref="" >IScriptRunner.IsScriptRunning()</see>
        bool IScriptRunner.IsScriptRunning(int processId)
        {
            Process process = _processes.GetValueOrDefault(processId);

            if (process == default(Process) )
            {
                string msg = $"Tried to find out, if script-process with process-ID {processId} is running, but no such process has been started.";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

            return ! process.HasExited;
        }

        #endregion

        //#####################################################################
        //## Helper methods
        //#####################################################################
        #region
        private ProcessStartInfo preapareProcessStartInfo(string scriptName, string scriptArgs)
        {
            FileInfo scriptPath = new FileInfo (
                    Path.Join(_scriptDirectory.FullName, $"{scriptName}{_scriptExtension}"));
            
            if (! scriptPath.Exists )
            {
                string msg = $"Script not existing: '{scriptPath.FullName}'";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

            string shellArgs = _shellArgsTemplate;
            shellArgs = shellArgs.Replace(PLACEHOLDER_SCRIPT_PATH, scriptPath.FullName);
            shellArgs = shellArgs.Replace(PLACEHOLDER_SCRIPT_ARGS, scriptArgs);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = _shell;
            startInfo.Arguments = shellArgs;
            startInfo.WorkingDirectory = scriptPath.Directory.FullName;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            return startInfo;
        }

        #endregion
    }
}
