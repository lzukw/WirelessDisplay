using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using WirelessDisplay.Common;
using WirelessDisplayServer.Services;
using Avalonia.Threading;


namespace WirelessDisplayServer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //#####################################################################
        // Constructor and private Fields
        //#####################################################################
        #region 

        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IServerController _serverController;
        private readonly string _ipAddress;

        public MainWindowViewModel( ILogger<MainWindowViewModel> logger, 
                                    IServerController serverController,
                                    string hostName,
                                    string iPAddress,
                                    List<UInt16> portNumbers )
        {
            _logger = logger;

            _serverController = serverController;
            // Attach Event-Handler: When the Server produces output, just add
            // the line to the Items in the ListBox for the Status-Log
            _serverController.ServerOutputReceived += async (o,e) => 
            { 
                if ( ! string.IsNullOrEmpty(e.Data) )
                {
                    await Dispatcher.UIThread.InvokeAsync(  () => { StatusLogLines.Add(e.Data);}  );
                }

            };

            PortNumbers = new ObservableCollection<UInt16>(portNumbers);
            if (PortNumbers.Count == 0)
            {
                logger?.LogWarning($"No Port-Numbers passed to the Constructor of MainWindowViewModel. Adding Port 80.");
                PortNumbers.Add(80);
            }

            _ipAddress = iPAddress;

            // pre-select first port-number in Combobox
            SelectedPortNumberIndex = 0;

            // IpAddressAndPort has already been initialized by the setter of
            // SelectedPortNumberIndex

            HostName = hostName;

            RestartButtonIsEnabled = true;

            StatusLogLines = new ObservableCollection<string>();

            // Finally perform a programatic click on thre "Reatart Server"-Button:
            ButtonRestartServer_Clicked();
        }

        #endregion

        //#####################################################################
        // Value-properties bound to controls in the view.
        //#####################################################################
        #region 

        // backup-field for RestartButtonIsEnabled
        private bool _restartButtonIsEnabled;
        /// <summary>
        /// Bound to the IsEnabled-Proberty of the view's Restart-Server-Button
        /// </summary>
        public bool RestartButtonIsEnabled
        {
            get => _restartButtonIsEnabled;
            set => this.RaiseAndSetIfChanged(ref _restartButtonIsEnabled, value);
        }


        // backup-field for IpAddress
        private string _ipAddressAndPort="";
        /// <summary> Bound to view's TextBox with the IP-Address. </summary>
        public string IpAddressAndPort
        {
            get => _ipAddressAndPort;
            set => this.RaiseAndSetIfChanged(ref _ipAddressAndPort, value);
        }


        // backup-field for HostName
        private string _hostName;
        /// <summary> Bound to the view's TextBox with the host-name. </summary>
        public string HostName
        {
            get => _hostName;
            set => this.RaiseAndSetIfChanged(ref _hostName, value);
        }


        /// <summary>
        /// Bound to the items of the view's ComboBox with the Port-Numbers.
        /// </summary>
        public ObservableCollection<UInt16> PortNumbers { get; }


        // backup-field for 
        private int _selectedPortNumberIndex;
        /// <summary>
        /// Bound to the index of the selected element in the ComboBox with the
        /// Port-Numbers. If another port is selected, also 'IPAddressAndPortNumber'
        /// has to be updated, and the Restart-Server-Button is enabled
        /// </summary>
        public int SelectedPortNumberIndex
        {
            get => _selectedPortNumberIndex;
            set
            {   
                // The setter is called, if another Port-Number is selected
                // in the ComboBox.
                this.RaiseAndSetIfChanged(ref _selectedPortNumberIndex, value);

                // Also update the shown IPAddressAndPortNumber
                if ( PortNumbers[_selectedPortNumberIndex] == 80)
                {
                    IpAddressAndPort = _ipAddress;
                }
                else
                {
                    IpAddressAndPort = $"{_ipAddress}:{PortNumbers[_selectedPortNumberIndex]}";
                }

                RestartButtonIsEnabled = true;
            }
        }


        /// <summary>
        /// Bound to the items in the ListBox showing the Log-output of the
        /// server.
        /// </summary>
        public ObservableCollection<string> StatusLogLines { get; }

        #endregion


        //#####################################################################
        // - React on Button-Clicks and Window-Close-Button. 
        // - Event-Handler, if sript-running-server produces output
        //#####################################################################
        #region

        /// <summary>
        /// Executed, when the view's Button "Restart Server" ist clicked
        /// </summary>
        public void ButtonRestartServer_Clicked()
        {
            // first stop a scripting-REST-API-server, if it is running.
            _serverController.StopServer();

            // Clear Log-output of the scripting-REST-API-server
            StatusLogLines.Clear();
            StatusLogLines.Add($"Logs starting {DateTime.Now}");

            // Try to start the scripting-REST-API-server
            _logger?.LogInformation("Trying to (re-)start the scripting-REST-API-server.");

            try 
            {
                _serverController.StartServerInBackground( PortNumbers[SelectedPortNumberIndex] );
            }
            catch (WDException e)
            {
                StatusLogLines.Add("ERROR: Could not start Script-Running-Server. Error-Message was:");
                StatusLogLines.Add(e.Message);
                return;
            }

            // The scripting-REST-API-server is now started. Disable the Restart-Server-Button.
            // (Selecting another Port in the ComboBox re-enables the Button).
            RestartButtonIsEnabled = false;
        }

        /// <summary>
        /// Executed, when the user closes the Window with the (X)-Button in the
        /// title-bar of the window. In order to reach this call, a line had to be 
        /// added in the code-behind of the view 'MainWindow.xaml.cs'
        /// </summary>
        public void OnWindowClosed()
        {
            _serverController.StopServer();
        }

        #endregion

    }
}
