using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WirelessDisplay.Common;

namespace WirelessDisplayServer.Services
{
    public class ServerController : IServerController
    {
        

        private ILogger<ServerController> _logger;

        private readonly FileInfo _pathToServerExecutable;
        private readonly string _argsTemplateForServerExecutable;

        private Process _serverProcess;

        public ServerController(ILogger<ServerController> logger, 
                                string pathToServerExecutable,
                                string argsTemplateForServerExecutable)
        {
            _logger = logger;
            
            _pathToServerExecutable = new FileInfo(pathToServerExecutable);
            if ( ! _pathToServerExecutable.Exists )
            {
                string msg = $"Path to server-executable not correct: '{_pathToServerExecutable.FullName}'";
                logger?.LogCritical(msg);
                throw new WDFatalException(msg);
            }

            _argsTemplateForServerExecutable = argsTemplateForServerExecutable;
            if ( ! _argsTemplateForServerExecutable.Contains(MAGICSTRINGS.PORT_PLACEHOLDER))
            {
                string msg = $"Template for server-arguments must contain '{MAGICSTRINGS.PORT_PLACEHOLDER}', but is: '{_argsTemplateForServerExecutable}'.";
                logger?.LogCritical(msg);
                throw new WDFatalException(msg);
            }
        }

        ///////////////////////////////////////////////////////////
        // Implentation of the interface
        ///////////////////////////////////////////////////////////
        #region 

        // For event-handling in C#, see
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
        // https://codeblog.jonskeet.uk/2015/01/30/clean-event-handlers-invocation-with-c-6/
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0071
        
        // private backup-"field" for event.
        protected DataReceivedEventHandler _serverOutputReceived;

        /// <see cref="WirelessDisplayServer.Services.IServerController"></see>
        event DataReceivedEventHandler IServerController.ServerOutputReceived
        {
            add { _serverOutputReceived += value; }
            remove { _serverOutputReceived -= value; }
        }

        // wrap event-invocations in a protected virtual method
        protected virtual void OnServerOutputReceived(DataReceivedEventArgs e)
        {
            _serverOutputReceived?.Invoke(this, e);
        }


        /// <see cref="WirelessDisplayServer.Services.IServerController.StartServerInBackground"></see>
        void IServerController.StartServerInBackground(UInt16 PortNo)
        {
            // Stop server, if it is still running.
            ((IServerController) this).StopServer();

            string commandLineArgs = _argsTemplateForServerExecutable
                            .Replace(MAGICSTRINGS.PORT_PLACEHOLDER, PortNo.ToString());

            _serverProcess = new Process();
            _serverProcess.StartInfo.FileName = _pathToServerExecutable.FullName;
            _serverProcess.StartInfo.Arguments = commandLineArgs;
            _serverProcess.StartInfo.WorkingDirectory = _pathToServerExecutable.Directory.FullName;
            _serverProcess.StartInfo.UseShellExecute = false;
            _serverProcess.StartInfo.CreateNoWindow = true;
            _serverProcess.StartInfo.RedirectStandardInput = false;
            _serverProcess.StartInfo.RedirectStandardOutput = true;
            _serverProcess.StartInfo.RedirectStandardError = true;
            
            // Forward raised events to the own event of this class.
            _serverProcess.OutputDataReceived += ( o, e) => { OnServerOutputReceived(e); };
            _serverProcess.ErrorDataReceived  += ( o, e) => { OnServerOutputReceived(e); };

            try
            {
                _serverProcess.Start();

                // These two lines are necessary: If they are not present, no
                // OutputDataReceived- and ErrorDataReceived-events occur. 
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                string msg = $"Could not start process '{_serverProcess.StartInfo.FileName} {_serverProcess.StartInfo.Arguments}': {e.Message}";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }

            //Wait a second, and check, if process crashed immediately
            bool exited = _serverProcess.WaitForExit(1000);
            if ( exited )
            {
                string msg = $"Process has exited immidiately: '{_serverProcess.StartInfo.FileName} {_serverProcess.StartInfo.Arguments}'.";
                _logger?.LogError(msg);
                throw new WDException(msg);
            }         
        }


        /// <see cref="WirelessDisplayServer.Services.IServerController.StopServer"></see>
        void IServerController.StopServer()
        {
            if ( _serverProcess != null && ! _serverProcess.HasExited )
            {
                _serverProcess.Kill ( entireProcessTree : true );
            }
            _serverProcess?.Dispose();
            _serverProcess = null;
        }


        #endregion
    }
}