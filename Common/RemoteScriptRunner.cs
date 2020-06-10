using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WirelessDisplay.Common.JsonObjects;

namespace WirelessDisplay.Common
{
    /// <summary>
    ///     A class for starting and stopping scripts (shell-scripts / batch-files)
    ///     and start running them in background on the remote computer. This
    ///     is done via POST-Requests to the ScriptingRestApiServer on the remote
    ///     machine.
    /// </summary>
    public class RemoteScriptRunner : IRemoteScriptRunner
    {
        //#####################################################################
        //## Private fields and constructor
        //#####################################################################
        #region 


        private readonly ILogger<RemoteScriptRunner> _logger;

        private readonly IPAddress _ipAddress;
        private readonly UInt16 _portNo;

        private static readonly HttpClient _client = new HttpClient();

        public RemoteScriptRunner(ILogger<RemoteScriptRunner> logger, 
                                  string iPAddress, UInt16 portNo
                                 )
        {
            _logger = logger;

            try
            {
                _ipAddress = IPAddress.Parse(iPAddress);
            }
            catch (FormatException)
            {
                string msg = $"Cannot convert to a valid IP-Address: {iPAddress}.";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

            _portNo = portNo;
        }

        #endregion

        //#####################################################################
        //## Implementing the API-methds (offered through the interface)
        //#####################################################################
        #region

        /// <see cref="WirelessDisplay.Common.IRemoteScriptRunner.RunAndWaitForScript"></see>
        async Task<Tuple<int,List<string>,List<string>>> IRemoteScriptRunner.RunAndWaitForScript( 
            string scriptName, string scriptArgs, string stdin, int timeoutMillis )
        {
            var postData = new StartOrRunScriptRequestData()
            {
                ScriptName = scriptName,
                ScriptArgs = scriptArgs,
                StdIn = stdin
            };

            // If an exception occurs, let it handle the caller
            RunOrStopScriptResponseData rd = await
            performPost<StartOrRunScriptRequestData,RunOrStopScriptResponseData>(postData, MAGICSTRINGS.RESTAPI_RUNSCRIPT);

            if ( ! rd.Success)
            {
                string msg = $"Could not run remote script '{scriptName} {scriptArgs}'. Error-Message from server: '{rd.ErrorMessage}'";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            return Tuple.Create(rd.ScriptExitCode, rd.StdoutLines, rd.StderrLines);
        }

        /// <see cref="WirelessDisplay.Common.IRemoteScriptRunner.StartScript"></see>
        async Task<int> IRemoteScriptRunner.StartScript( string scriptName, string scriptArgs, string stdin, 
                                     int shortTimeoutMillis)
        {
            var postData1 = new StartOrRunScriptRequestData()
            {
                ScriptName = scriptName,
                ScriptArgs = scriptArgs,
                StdIn = stdin
            };

            // If an exception occurs, let it handle the caller
            StartScriptResponseData rd = await
                performPost<StartOrRunScriptRequestData,StartScriptResponseData>(postData1, 
                MAGICSTRINGS.RESTAPI_STARTSCRIPT);

            if ( ! rd.Success)
            {
                string msg = $"Could not start remote script '{scriptName} {scriptArgs}'. Error-Message from server: '{rd.ErrorMessage}'";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            int processId = rd.ProcessId;

            // Wait the givem time (alltough the server also has waited before)
            await Task.Delay(shortTimeoutMillis);

            bool startedSuccessfully = await ((IRemoteScriptRunner) this).IsScriptRunning(
                        processId);

            if ( ! startedSuccessfully )
            {
                string msg = $"The started script '{scriptName} {scriptArgs}' with process-ID {processId} terminated immediately.";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            return processId;
        }


        /// <see cref="WirelessDisplay.Common.IRemoteScriptRunner.StopScript"></see>
        async Task<Tuple<int,List<string>,List<string>>> IRemoteScriptRunner.StopScript( 
            int processId )
        {
            var postData = new QueryOrStopScriptRequestData()
            {
                ProcessId = processId
            };

            // If an exception occurs, let it handle the caller
            RunOrStopScriptResponseData rd = await
                performPost<QueryOrStopScriptRequestData,RunOrStopScriptResponseData>(
                                postData, MAGICSTRINGS.RESTAPI_STOPSCRIPT);

            if ( ! rd.Success)
            {
                string msg = $"Could not stop remote script with process-ID {processId}. Error-Message from server: '{rd.ErrorMessage}'";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            return Tuple.Create(rd.ScriptExitCode, rd.StdoutLines, rd.StderrLines); 
        }


        /// <see cref="WirelessDisplay.Common.IRemoteScriptRunner.IsScriptRunning"></see>
        async Task<bool> IRemoteScriptRunner.IsScriptRunning(int processId)
        {
            var postData = new QueryOrStopScriptRequestData()
            {
                ProcessId = processId
            };
                        
            QueryScriptResponseData rd = await 
                performPost<QueryOrStopScriptRequestData,QueryScriptResponseData>(postData, 
                MAGICSTRINGS.RESTAPI_IS_SCRIPT_RUNNING);

            if ( ! rd.Success)
            {
                string msg = $"Could query state of script with process-ID {processId}. Error-Message from server: '{rd.ErrorMessage}'";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            return rd.IsRunning;
        }

        #endregion


        /////////////////////////////////////////////////////////
        // Helper methods
        /////////////////////////////////////////////////////////
        #region 

        async Task<TResponse> performPost<TRequest,TResponse>(TRequest postData, string lastPartOfApiPath)
        {
            // This never throws an exception
            string postString = JsonSerializer.Serialize(postData);

            string uriString = $"http://{_ipAddress}:{_portNo}/{MAGICSTRINGS.RESTAPI_MAIN_PATH}/{lastPartOfApiPath}";

            HttpResponseMessage response;

            try
            {   
                var uri = new Uri(uriString);
                var content = new StringContent(postString, System.Text.Encoding.UTF8, "application/json");
                
                response = await _client.PostAsync( uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                string msg = $"SPOST-Request with data '{postString}' to '{uriString}' failed. Errormessage: '{e.Message}'";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            string responseString = await response.Content.ReadAsStringAsync();
            TResponse responseObject;
            try
            {
                responseObject = JsonSerializer.Deserialize<TResponse>(responseString);
            }
            catch (JsonException)
            {
                string msg = $"Could not convert the the POST-Response '{responseString}' to an object of type '{typeof(TResponse)}'";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            return responseObject;
        }

        #endregion

    }
}