using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using WirelessDisplay.Common;
using Microsoft.Extensions.Logging;

namespace WirelessDisplayClient.Services
{
    /// <summary>
    /// Provides all the services needed by the MainWindow-viewmodel.
    /// </summary>
    public class WDClientServices : IWDClientServices
    {

        //#####################################################################
        //## Private fields and constructor
        //#####################################################################
        #region

        private readonly ILogger<WDClientServices> _logger;
        private readonly ILocalScriptRunner _localScriptRunner;
        private readonly IRemoteScriptRunner _remoteScriptRunner;
        private readonly string _scriptNameManageScreenResolutions;
        private readonly string _scriptArgsManageScreenResolutions;
        private readonly string _scriptNameStartStreamingSink;
        private readonly string _scriptArgsStartStreamingSink;
        private readonly string _scriptNameStartStreamingSource;
        private readonly string _scriptArgsStartStreamingSource;
        private readonly string _scriptNamePreventDisplayBlanking;
        private readonly string _scriptArgsPreventDisplayBlanking;

        private readonly string _inialLocalScreenResolution;

        // Set, when ConnectToServer() is called.
        private string _initialRemoteScreenResolution;

        /// <summary>
        /// The process-ID of the local streaming-source. Zero means, that
        /// no local process has been started.
        /// </summary>
        private int _processIdStramingSource = 0;

        /// <summary>
        /// The process-ID of the remote streaming-sink. Zero means, that
        /// no remote process has been started.
        /// </summary>
        private int _processIdStreamingSink = 0;

        /// <summary>
        /// The process-ID of the remote sript that prevents the display
        /// from blanking on the remote computer. Zero means, that no 
        /// remote process has been started.
        /// </summary>
        private int _processIdPreventDisplayBlanking = 0;

        public WDClientServices( ILogger<WDClientServices> logger,
                                ILocalScriptRunner localScriptRunner,
                                IRemoteScriptRunner remoteScriptRunner,
                                string scriptNameManageScreenResolutions,
                                string scriptArgsManageScreenResolutions,
                                string scriptNameStartStreamingSink,
                                string scriptArgsStartStreamingSink,
                                string scriptNameStartStreamingSource,
                                string scriptArgsStartStreamingSource,
                                string scriptNamePreventDisplayBlanking,
                                string scriptArgsPreventDisplayBlanking )
        {
            _logger = logger;
            _localScriptRunner = localScriptRunner;
            _remoteScriptRunner = remoteScriptRunner;
            _scriptNameManageScreenResolutions = scriptNameManageScreenResolutions;
            _scriptArgsManageScreenResolutions = scriptArgsManageScreenResolutions;
            _scriptNameStartStreamingSink = scriptNameStartStreamingSink;
            _scriptArgsStartStreamingSink = scriptArgsStartStreamingSink;
            _scriptNameStartStreamingSource = scriptNameStartStreamingSource;
            _scriptArgsStartStreamingSource = scriptArgsStartStreamingSource;
            _scriptNamePreventDisplayBlanking = scriptNamePreventDisplayBlanking;
            _scriptArgsPreventDisplayBlanking = scriptArgsPreventDisplayBlanking;

            _inialLocalScreenResolution = 
                    ((IWDClientServices) this).GetCurrentLocalScreenResolution();
        }

        #endregion


        //#####################################################################
        // Implementation of the interface 
        // - manage local and remote screen-resolutions
        //#####################################################################
        #region

        /// <see cref="IWDClientServices.ConnectToServer" ></see>
        async Task IWDClientServices.ConnectToServer(string ipAddress, UInt16 port)
        {
            _remoteScriptRunner.SetIpAddressAndPort(ipAddress, port);            
            await ((IWDClientServices) this).GetInitalRemoteScreenResolution();
        }


        /// <see cref="IWDClientServices.GetInitalLocalScreenResolution" ></see>
        string IWDClientServices.GetInitalLocalScreenResolution()  => _inialLocalScreenResolution;


        /// <see cref="IWDClientServices.GetCurrentLocalScreenResolution" ></see>
        string IWDClientServices.GetCurrentLocalScreenResolution() 
        {
            string scriptArgs = _scriptArgsManageScreenResolutions;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_ACTION, 
                                                MagicStrings.ACTION_GET);

            List<string> stdoutLines = runLocalManageScreenResolutionsScript(scriptArgs);
            return findFirstScreenResolution(stdoutLines);
        }


        /// <see cref="IWDClientServices.GetAvailableLocalScreenResolutions"></see>
        List<string> IWDClientServices.GetAvailableLocalScreenResolutions()
        {
            string scriptArgs = _scriptArgsManageScreenResolutions;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_ACTION, 
                                                MagicStrings.ACTION_ALL);

            List<string> stdoutLines = runLocalManageScreenResolutionsScript(scriptArgs);
            List<string> allResolutions = findAllScreenResolutions(stdoutLines);
            if (allResolutions.Count == 0)
            {
                string msg = $"Could not get local screen-resolutions. Script returned: '{string.Join(';', stdoutLines)}'";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }
            return allResolutions;
        }

        /// <see cref="IWDClientServices.SetLocalScreenResolution(string)"></see>
        void IWDClientServices.SetLocalScreenResolution(string screenResolution)
        {
            string scriptArgs = _scriptArgsManageScreenResolutions;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_ACTION, 
                                                MagicStrings.ACTION_SET);
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_SCREEN_RESOLUTION, 
                                                screenResolution);
            
            runLocalManageScreenResolutionsScript(scriptArgs);
        }

        
        /// <see cref="IWDClientServices.GetInitalRemoteScreenResolution"></see>
        async Task<string> IWDClientServices.GetInitalRemoteScreenResolution() 
        {  
            if (_initialRemoteScreenResolution == null)
            {
                _initialRemoteScreenResolution = 
                        await ((IWDClientServices) this).GetCurrentRemoteScreenResolution();
            }
            
            return _initialRemoteScreenResolution;
        }


        /// <see cref="IWDClientServices.GetCurrentRemoteScreenResolution"></see>
        async Task<string> IWDClientServices.GetCurrentRemoteScreenResolution() 
        {
            string scriptArgs = _scriptArgsManageScreenResolutions;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_ACTION, MagicStrings.ACTION_GET);

            List<string> outputLines = await runRemoteManageScreenResolutionsScript(scriptArgs);

            string currentResolution = findFirstScreenResolution(outputLines);

            return currentResolution;
        }

        /// <see cref="IWDClientServices.GetAvailableRemoteScreenResolutions"></see>
        async Task<List<string>> IWDClientServices.GetAvailableRemoteScreenResolutions()
        {
            string scriptArgs = _scriptArgsManageScreenResolutions;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_ACTION, 
                                                MagicStrings.ACTION_ALL);

            List<string> stdoutLines = await runRemoteManageScreenResolutionsScript(scriptArgs);
            List<string> allResolutions = findAllScreenResolutions(stdoutLines);
            if (allResolutions.Count == 0)
            {
                string msg = $"Could not get remote screen-resolutions. Script returned: '{string.Join(';', stdoutLines)}'";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }
            return allResolutions;
        }

        /// <see cref="IWDClientServices.SetRemoteScreenResolution"></see>
        async Task IWDClientServices.SetRemoteScreenResolution(string screenResolution)
        {
            string scriptArgs = _scriptArgsManageScreenResolutions;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_ACTION, 
                                                MagicStrings.ACTION_SET);
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_SCREEN_RESOLUTION, 
                                                screenResolution);

            await runRemoteManageScreenResolutionsScript(scriptArgs);    
        }

        #endregion

        //#####################################################################
        // Implementation of the interface 
        // - start and stop local streaming-source and remote streaming-sink
        //#####################################################################
        #region

        /// <see cref="IWDClientServices.StartLocalStreamingSource"></see>
        void IWDClientServices.StartLocalStreamingSource( string streamType,
                                     string remoteIpAddress,
                                     UInt16 port,
                                     string streamResolution )
        {
            // First check the given remoteIpAddress
            IPAddress dummy;
            if ( string.IsNullOrWhiteSpace(remoteIpAddress) || 
                 ! IPAddress.TryParse( remoteIpAddress, out dummy ) )
            {
                string msg = $"This is not a valid IP-Address: '{remoteIpAddress}'";
                _logger?.LogWarning(msg);
                throw new WDException(msg);
            }

            // Then stop eventually running streaming-source
            ((IWDClientServices) this).StopLocalStreamingSource();

            // Run the local script to start the streaming-source
            string scriptArgs = _scriptArgsStartStreamingSource;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_STREAMING_TYPE, streamType);
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_IP, remoteIpAddress);
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_PORT, port.ToString());
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_SCREEN_RESOLUTION, !string.IsNullOrEmpty(streamResolution) ? streamResolution : "null");

            // Exceptions have to be handled by the caller.
            // Store the process-ID of the started process in _processIdStramingSource
            _processIdStramingSource = _localScriptRunner.StartScript(
                                _scriptNameStartStreamingSource, scriptArgs);

            _logger?.LogInformation($"Started local streaming-source of type '{streamType}' to '{remoteIpAddress}:{port}' using stream-resolution {streamResolution}. Process-Id={_processIdStramingSource}");
        }
        
        /// <see cref="IWDClientServices.StopLocalStreamingSource"></see>
        void IWDClientServices.StopLocalStreamingSource()
        {
            // A process-ID of 0 means, that the process has not been started yet.
            if (_processIdStramingSource != 0)
            {
                _localScriptRunner.StopScript(_processIdStramingSource);
                _logger?.LogInformation($"Stopped streaming-source with process-ID {_processIdStramingSource}");
                _processIdStramingSource = 0;
            }
            
        }

        /// <see cref="IWDClientServices.StartRemoteStreamingSink"></see>
        async Task IWDClientServices.StartRemoteStreamingSink( string streamType, UInt16 port)
        {
            // First stop eventually running remote streaming-sink
            await ((IWDClientServices) this).StopRemoteStreamingSink();

            string scriptArgs = _scriptArgsStartStreamingSink;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_STREAMING_TYPE, streamType);
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_PORT, port.ToString());
            
            // Exceptions have to be handled by the caller.
            // Store the process-ID of the started process in _processIdStreamingSink
            _processIdStreamingSink = await _remoteScriptRunner.StartScript( 
                        _scriptNameStartStreamingSink, scriptArgs);

            _logger?.LogInformation($"Started remote streaming-sink of type '{streamType}', listening on port {port}. Process-Id={_processIdStreamingSink}.");
        }

        /// <see cref="IWDClientServices.StopRemoteStreamingSink"></see>
        async Task IWDClientServices.StopRemoteStreamingSink()
        {
            // A process-ID of 0 means, that the remote process has not been started yet.
            if (_processIdStreamingSink != 0)
            {
                await _remoteScriptRunner.StopScript(_processIdStreamingSink);
                _logger?.LogInformation($"Stopped remote streaming-sink with process-ID {_processIdStreamingSink}");
                _processIdStreamingSink = 0;
            }
        }

        #endregion


        //#####################################################################
        // Implementation of the interface 
        // - start and stop remote script to prevent display from blanking.
        //#####################################################################
        #region

        /// <see cref="IWDClientServices.StartRemotePreventDisplayBlanking"></see>
        async Task IWDClientServices.StartRemotePreventDisplayBlanking( int seconds )
        {
            // First stop eventually running remote script
            await ((IWDClientServices) this).StopRemotePreventDisplayBlanking();

            string scriptArgs = _scriptArgsPreventDisplayBlanking;
            scriptArgs = scriptArgs.Replace(MagicStrings.PLACEHOLDER_SECONDS, 
                            seconds.ToString());

            // Exceptions have to be handled by the caller.
            // Store the process-ID of the started process in _processIdPreventDisplayBlanking
            _processIdPreventDisplayBlanking = await _remoteScriptRunner.StartScript(
                            _scriptNamePreventDisplayBlanking, scriptArgs );

            _logger?.LogInformation($"Started remote script to prevent display from blanking for {seconds} seconds. Process-ID={_processIdPreventDisplayBlanking}");
        }


        /// <see cref="IWDClientServices.StartRemotePreventDisplayBlanking"></see>
        async Task IWDClientServices.StopRemotePreventDisplayBlanking()
        {
            if (_processIdPreventDisplayBlanking != 0 )
            {
                await _remoteScriptRunner.StopScript(_processIdPreventDisplayBlanking);
                _logger?.LogInformation($"Stopped remote scipt to prevent the display from blanking with process-ID {_processIdPreventDisplayBlanking}");
                _processIdPreventDisplayBlanking = 0;
            }
        }

        #endregion

        //#####################################################################
        // Helper methods
        //#####################################################################
        #region

        /// <summary>
        /// Runs the script 'manageScreenResolutions' on the local computer.
        /// </summary>
        /// <param name="scriptArgs"> 
        /// The command-line-arguments passed to the script
        /// </param>
        /// <param name="timeoutMillis">
        /// The maximum time to wait until the script returns  in milliseconds.
        /// </param>
        /// <returns>
        /// The lines written to stdout by the script.
        /// </returns>
        private List<string> runLocalManageScreenResolutionsScript(
                        string scriptArgs, int timeoutMillis=10000)
        {
            // if an exception occurs, let it handle the caller
            int exitCode;
            List<string> stdoutLines;
            List<string> stderrLines;
            ( exitCode, stdoutLines, stderrLines ) = _localScriptRunner
                    .RunAndWaitForScript( _scriptNameManageScreenResolutions, scriptArgs, 
                                          timeoutMillis : timeoutMillis);

            if (exitCode != 0)
            {
                string msg = $"Local script returned with exit-code {exitCode}: '{_scriptNameManageScreenResolutions} {scriptArgs}'. Standard-error-outut: '{string.Join(';', stderrLines)}'";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

            return stdoutLines;
        }


        /// <summary>
        /// Runs the script 'manageScreenResolutions' on the remote computer.
        /// </summary>
        /// <param name="scriptArgs"> 
        /// The command-line-arguments passed to the script
        /// </param>
        /// <param name="timeoutMillis">
        /// The maximum time to wait until the script returns  in milliseconds.
        /// </param>
        /// <returns>
        /// The lines written to stdout by the script.
        /// </returns>
        private async Task<List<string>> runRemoteManageScreenResolutionsScript (
                        string scriptArgs)
        {
            // if an exception occurs, let it handle the caller
            int exitCode;
            List<string> stdoutLines;
            List<string> stderrLines;
            ( exitCode, stdoutLines, stderrLines ) = await _remoteScriptRunner.RunAndWaitForScript(
                _scriptNameManageScreenResolutions, scriptArgs);
            
            if (exitCode != 0)
            {
                string msg = $"Remote script returned with exit-code {exitCode}: '{_scriptNameManageScreenResolutions} {scriptArgs}'. Standard-error-outut: '{string.Join(';', stderrLines)}'";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

            return stdoutLines;
        }

        /// <summary>
        /// Parse the given List of strings for screen-resolutions, and return
        /// the first found screen-resolution.
        /// </summary>
        /// <param name="stdoutLines"> The strings to be parsed. </param>
        /// <returns> The first screen-resolution found. </returns>
        /// <exception cref="WDException"> 
        /// If no screen-resolution is found.
        /// </exception>
        private string findFirstScreenResolution(IEnumerable<string> stdoutLines)
        {
            // Parse stdoutLines and return first line containing a screen-resolution
            foreach (string outputLine in stdoutLines)
            { 
                MatchCollection mc = Regex.Matches(outputLine, @"[^\d]*(\d+x\d+).*");
                if (mc.Count==1 && mc[0].Groups.Count==2)
                {
                    return mc[0].Groups[1].ToString();
                }
            }

            // No stdoutLine containing a screen-resolution found. Throw exception
            string msg = $"Could not parse valid screen-resolution from script-output: '{string.Join(';', stdoutLines)}'";
            _logger?.LogError(msg);
            throw new WDException(msg);
        }


        /// <summary>
        /// Parse the given List of strings for screen-resolutions.
        /// </summary>
        /// <param name="stdoutLines"> The strings to be parsed. </param>
        /// <returns> 
        /// All found screen-resolutions, or an empty list, if no
        /// screen-resolution was found.
        /// </returns>
        private List<string> findAllScreenResolutions(IEnumerable<string> stdoutLines)
        {
            var allResolutions = new List<string>();
            foreach (string outputLine in stdoutLines)
            { 
                MatchCollection mc = Regex.Matches(outputLine, @"[^\d]*(\d+x\d+).*");
                if (mc.Count==1 && mc[0].Groups.Count==2)
                {
                    allResolutions.Add( mc[0].Groups[1].ToString() );
                }
            }
            return allResolutions;
        }

        #endregion
    }
}