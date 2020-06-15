using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WirelessDisplay.Common;
using WirelessDisplay.Common.JsonObjects;

namespace ScriptingRestApiServer.Controllers
{
    [Route(MagicStrings.RESTAPI_MAIN_PATH)]
    [ApiController]
    public class ScriptRunnerController : ControllerBase
    {
        private ILogger<ScriptRunnerController> _logger { get; }
        private ILocalScriptRunner _scriptRunner { get; }

        public ScriptRunnerController(  ILogger<ScriptRunnerController> logger,
                                        ILocalScriptRunner scriptRunner)
        {
            // logger and scriptRunner are injected by Dependcy-Injection 
            // by "the runtime" (Program.cs and Startup.cs).
            _logger = logger;
            _scriptRunner = scriptRunner;    
        }

        // POST: api/ScriptRunner/RunScript
        // Example from Linux (using 127.0.0.1:6000 as server-IP-address and server-port): 
        // curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ScriptName" : "testscript_for_run", "ScriptArgs" : "arg1 arg2 arg3 arg4", "StdIn" : "\n" }' http://127.0.0.1:6000/api/ScriptRunner/RunScript
        [HttpPost(MagicStrings.RESTAPI_RUNSCRIPT)]
        public RunOrStopScriptResponseData Post_RunScript([FromBody] StartOrRunScriptRequestData requestData)
        {
            int exitCode;
            List<string>outputLines;
            List<string>errorLines;
             
            try
            {
                (exitCode, outputLines, errorLines) = 
                     _scriptRunner.RunAndWaitForScript(  requestData.ScriptName,
                                                         requestData.ScriptArgs,
                                                         requestData.StdIn);
            }
            catch (WDException e)
            {
                return new RunOrStopScriptResponseData() 
                            {
                                Success = false,
                                ErrorMessage = e.Message,
                                ScriptExitCode = -1,
                                StdoutLines = new List<string>(),
                                StderrLines = new List<string>()
                            };
            }
            
            return new RunOrStopScriptResponseData()
                        {
                            Success = true,
                            ErrorMessage = "",
                            ScriptExitCode = exitCode,
                            StdoutLines = outputLines,
                            StderrLines = errorLines
                        };
        }


        // POST: api/ScriptRunner/StartScript
        // Example from Linux (using 127.0.0.1:6000 as server-IP-address and server-port): 
        // curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ScriptName" : "testscript_for_start_stop", "ScriptArgs" : "arg1 arg2 arg3 arg4", "StdIn" : "\n" }' http://127.0.0.1:6000/api/ScriptRunner/StartScript
        [HttpPost(MagicStrings.RESTAPI_STARTSCRIPT)]
        public StartScriptResponseData Post_StartScript([FromBody] StartOrRunScriptRequestData requestData)
        {
            int processId;

            try 
            {
                processId = _scriptRunner.StartScript(   requestData.ScriptName,
                                                         requestData.ScriptArgs,
                                                         requestData.StdIn);
            }
            catch (WDException e)
            {
                return new StartScriptResponseData()
                            {
                                Success = false,
                                ErrorMessage = e.Message,
                                ProcessId = -1
                            };
            }

            return new StartScriptResponseData()
                        {
                            Success = true,
                            ErrorMessage = "",
                            ProcessId = processId
                        };

        }

        // POST: api/ScriptRunner/IsScriptRunning
        // Example from Linux (using 127.0.0.1:6000 as server-IP-address and server-port):
        // Further assuming a process-Id of 12345 returned from previous StartScript-Request
        // curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ProcessId" : 12345 }' http://127.0.0.1:6000/api/ScriptRunner/IsScriptRunning
        [HttpPost(MagicStrings.RESTAPI_IS_SCRIPT_RUNNING)]
        public QueryScriptResponseData Post_IsScriptRunning([FromBody] QueryOrStopScriptRequestData requestData)
        {
            bool isRunning;
            try
            {
                isRunning = _scriptRunner.IsScriptRunning(requestData.ProcessId);
            }
            catch (WDException e)
            {
                return new QueryScriptResponseData()
                            {
                                Success = false,
                                ErrorMessage = e.Message,
                                IsRunning = false
                            };
            }

            return new  QueryScriptResponseData()
                        {
                            Success = true,
                            ErrorMessage = "",
                            IsRunning = isRunning
                        };
        }


        // POST: api/ScriptRunner/StopScript
        // Example from Linux (using 127.0.0.1:6000 as server-IP-address and server-port):
        // Further assuming a process-Id of 12345 returned from previous StartScript-Request
        // curl -v POST -H "Accept: application/json" -H "Content-Type: application/json" --data '{ "ProcessId" : 12345 }' http://127.0.0.1:6000/api/ScriptRunner/StopScript
        [HttpPost(MagicStrings.RESTAPI_STOPSCRIPT)]
        public RunOrStopScriptResponseData Post_StopScript([FromBody] QueryOrStopScriptRequestData requestData)
        {
            int exitCode;
            List<string>outputLines;
            List<string>errorLines;
             
            try
            {
                (exitCode, outputLines, errorLines) = 
                     _scriptRunner.StopScript(  requestData.ProcessId);
            }
            catch (WDException e)
            {
                return new RunOrStopScriptResponseData() 
                            {
                                Success = false,
                                ErrorMessage = e.Message,
                                ScriptExitCode = -1,
                                StdoutLines = new List<string>(),
                                StderrLines = new List<string>()
                            };
            }
            
            return new RunOrStopScriptResponseData()
                        {
                            Success = true,
                            ErrorMessage = "",
                            ScriptExitCode = exitCode,
                            StdoutLines = outputLines,
                            StderrLines = errorLines
                        };
        }

    }
}