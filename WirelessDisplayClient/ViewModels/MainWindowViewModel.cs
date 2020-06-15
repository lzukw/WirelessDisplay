using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Extensions.Logging;
using WirelessDisplay.Common;
using WirelessDisplayClient.Services;

namespace WirelessDisplayClient.ViewModels
{
    /// <summary>
    /// Viewmodel of the Main-Window. Public properties are bound automatically
    /// to the Main-Window-view defined in MainWindow.xaml and MainWindow.xaml.cs
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        //#####################################################################
        // Constructor and private Fields
        //#####################################################################
        #region

        private readonly ILogger<MainWindowViewModel> _logger;

        private readonly IWDClientServices _wdClientServices;
        private readonly UInt16 _preferredServerPort;
        private readonly UInt16 _preferredStreamingPort;
        private readonly int _preferredLocalScreenWidth;
        private readonly int _preferredRemoteScreenWidth;

        private readonly int _indexOfpreferredStreamingType;


        public MainWindowViewModel( ILogger<MainWindowViewModel> logger,
                  IWDClientServices wdClientServices, 
                  UInt16 preferredServerPort = 80,
                  UInt16 preferredStreamingPort = 5500,
                  int preferredLocalScreenWidth = 1024,
                  int preferredRemoteScreenWidth = 1024,
                  int indexOfpreferredStreamingType = 0)
        {
            _logger = logger;
            _wdClientServices = wdClientServices;
            _preferredServerPort = preferredServerPort;
            _preferredStreamingPort = preferredStreamingPort;
            _preferredLocalScreenWidth = preferredLocalScreenWidth;
            _preferredRemoteScreenWidth = preferredRemoteScreenWidth;

            // Initialize bound properties
            ServerPort = _preferredServerPort;
            InitialLocalScreenResolution = _wdClientServices.GetInitalLocalScreenResolution();
            CurrentLocalScreenResolution = _wdClientServices.GetCurrentLocalScreenResolution();
            //InitialRemoteScreenResolution = _wdClientServices.GetInitalRemoteScreenResolution();
            //CurrentRemoteScreenResolution = await _wdClientServices.GetCurrentRemoteScreenResolution();
            
            AvailableLocalScreenResolutions = new ObservableCollection<string>();
            SelectedLocalScreenResolutionIndex = -1;
            AvailableRemoteScreenResolutions = new ObservableCollection<string>();
            SelectedRemoteScreenResolutionIndex = -1;
            
            StreamingTypes = new ObservableCollection<string>( new[] 
            {
                MagicStrings.STREAMING_METHOD_VNC,
                MagicStrings.STREAMING_METHOD_FFMPEG
            });

            if (indexOfpreferredStreamingType >= StreamingTypes.Count)
            {
                indexOfpreferredStreamingType = 0;
            }
            _indexOfpreferredStreamingType = indexOfpreferredStreamingType;
            SelectedStreamingTypeIndex = _indexOfpreferredStreamingType;

            StreamingPort = _preferredStreamingPort;
        }

        #endregion

        //#####################################################################
        // Boolean properties for enabling and disabling controls of the 
        // MainWindow
        //#####################################################################
        #region 

        // Backup-field for property ConnectionEstablished.
        private bool _connectionEstablished = false; 

        /// <summary>
        /// ConnectionEstablished is used to enable/disable the "Connect"-
        //  and "Disconnect"-Button and the TextBox for the IP-Address.
        /// </summary>
        public bool ConnectionEstablished
        {
            get => _connectionEstablished;
            set => this.RaiseAndSetIfChanged(ref _connectionEstablished, value);
        }


        // Backup-field for property StreamStarted
        private bool _streamStarted = false;

        /// <summary>
        /// StreamStarted is false after connecting and before starting the stream.
        /// It becomes true after starting the stream.
        /// </summary>
        /// <value></value>
        public bool StreamStarted
        {
            get => _streamStarted;
            set => this.RaiseAndSetIfChanged(ref _streamStarted, value);
        }

        #endregion


        //#####################################################################
        // Value-properties bound to controls 
        //#####################################################################
        #region

        private string _ipAddress="";

        /// <summary> Bound to TextBox with IP-Address.async  </summary>
        public string IpAddress
        {
            get => _ipAddress;
            set => this.RaiseAndSetIfChanged(ref _ipAddress, value);
        }

        // Backup-Field for ServerPort
        private UInt16 _serverPort;

        /// <summary>
        /// Bound to the NumericUpDown with the port-Number of the
        /// Scripting-REST-API-Server
        /// </summary>
        public UInt16 ServerPort
        {
            get => _serverPort;
            set => this.RaiseAndSetIfChanged(ref _serverPort, value);
        }


        // Backup-field for InitialLocalScreenResolution
        private string _initialLocalScreenResolution;

        /// <summary>
        /// Bound to TextBlock containing initial local screen-resolution
        /// </summary>
        public string InitialLocalScreenResolution
        {
            get => _initialLocalScreenResolution;
            set => this.RaiseAndSetIfChanged(ref _initialLocalScreenResolution, value);
        }


        // Backup-field for CurrentLocalScreenResolution
        private string _currentLocalScreenResolution;

        /// <summary>
        /// Bound to TextBlock containing current local screen-resolution
        /// </summary>
        public string CurrentLocalScreenResolution
        {
            get => _currentLocalScreenResolution;
            set => this.RaiseAndSetIfChanged(ref _currentLocalScreenResolution, value);
        }


        // Backup-field for InitialRemoteScreenResolution
        private string _initialRemoteScreenResolution;

        /// <summary>
        /// Bound to TextBlock containing initial remote screen-resolution
        /// </summary>
        public string InitialRemoteScreenResolution
        {
            get => _initialRemoteScreenResolution;
            set => this.RaiseAndSetIfChanged(ref _initialRemoteScreenResolution, value);
        }


        // Backup-field for CurrentRemoteScreenResolution
        private string _currentRemoteScreenResolution;

        /// <summary>
        /// Bound to TextBlock containing current remote screen-resolution
        /// </summary>
        public string CurrentRemoteScreenResolution
        {
            get => _currentRemoteScreenResolution;
            set => this.RaiseAndSetIfChanged(ref _currentRemoteScreenResolution, value);
        }


        /// <summary>
        /// Bound to items of the ComboBox with availabe screen-resolutions on
        /// the local computer.
        /// </summary>
        public ObservableCollection<string> AvailableLocalScreenResolutions { get; } 


        // Backup-field for SelectedLocalScreenResolutionIndex
        private int _selectedLocalScreenResolutionIndex = -1;

        /// <summary>
        /// Bound to the index of the selected item in the ComboBox with availabe 
        /// screen-resolutions on the local computer (-1 if no item is selected).
        /// </summary>
        public int SelectedLocalScreenResolutionIndex
        {
            get => _selectedLocalScreenResolutionIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedLocalScreenResolutionIndex, value);
        }

        /// <summary>
        /// Bound to items of the ComboBox with availabe screen-resolutions on
        /// the remote computer.
        /// </summary>
        public ObservableCollection<string> AvailableRemoteScreenResolutions { get; } 


        // Backup-field for SelectedRemoteScreenResolutionIndex
        private int _selectedRemoteScreenResolutionIndex = -1;

        /// <summary>
        /// Bound to the index of the selected item in the ComboBox with availabe 
        /// screen-resolutions on the remote computer (-1 if no item is selected).
        /// </summary>
        public int SelectedRemoteScreenResolutionIndex
        {
            get => _selectedRemoteScreenResolutionIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedRemoteScreenResolutionIndex, value);
        }


        /// <summary>
        /// Bound to items of the ComboBox with the streaming-methods
        /// </summary>
        public ObservableCollection<string> StreamingTypes { get; } 


        // Backup-field for SelectedStreamingTypeIndex
        private int _selectedStreamingTypeIndex = -1;

        /// <summary>
        /// Bound to the index of the selected item in the ComboBox with the
        /// streaming-methods.
        /// </summary>
        public int SelectedStreamingTypeIndex
        {
            get => _selectedStreamingTypeIndex;
            set => this.RaiseAndSetIfChanged(ref _selectedStreamingTypeIndex, value);
        }


        // Backup-field for StreamingPort
        private UInt16 _streamingPort;

        /// <summary>
        /// Bound to the NumericUpDown with the port-Number used for streaming.
        /// </summary>
        public UInt16 StreamingPort
        {
            get => _streamingPort;
            set => this.RaiseAndSetIfChanged(ref _streamingPort, value);
        }


        /// <summary>
        /// Bound to the items in the ListBox showing the Log-output.
        /// </summary>
        public ObservableCollection<string> StatusLogLines { get; }
                = new ObservableCollection<string>();


        #endregion


        //#####################################################################
        // React on Button-Clicks and Window-Close-Button. 
        //#####################################################################
        #region

        public async Task ButtonConnect_Click()
        {
            // Perform some simple error-checks:
            if (String.IsNullOrWhiteSpace(IpAddress))
            {
                StatusLogLines.Add("Please enter a valid IP-Address first");
                return;
            }

            // Connect to server
            string msg;
            try
            {
                await _wdClientServices.ConnectToServer(IpAddress, ServerPort);
            }
            catch(WDException e)
            {
                msg = $"Could not connect to server at '{IpAddress}:{ServerPort}': IP-Address and port valid? Server running? Inner error-message: {e.Message}";
                _logger?.LogInformation(msg);
                StatusLogLines.Add(msg);
                return;
            }

            // Update TectBlocks with initial and current screen-resolutions.
            InitialLocalScreenResolution = 
                    _wdClientServices.GetInitalLocalScreenResolution();
            CurrentLocalScreenResolution = 
                    _wdClientServices.GetCurrentLocalScreenResolution();
            InitialRemoteScreenResolution = await
                    _wdClientServices.GetInitalRemoteScreenResolution();
            CurrentRemoteScreenResolution = await
                    _wdClientServices.GetCurrentRemoteScreenResolution();

            // Update ComboBoxes with available screen-resoutions.
            AvailableLocalScreenResolutions.Clear();
            foreach (string screenResolution in 
                        _wdClientServices.GetAvailableLocalScreenResolutions() )
            {
                AvailableLocalScreenResolutions.Add(screenResolution);
            }

            AvailableRemoteScreenResolutions.Clear();
            foreach (string screenResolution in await 
                        _wdClientServices.GetAvailableRemoteScreenResolutions() )
            {
                AvailableRemoteScreenResolutions.Add(screenResolution);
            }

            // Pre-select preferred screen-resolutions for streaming
            SelectedLocalScreenResolutionIndex = indexOfNearestResolution(
                                AvailableLocalScreenResolutions, _preferredLocalScreenWidth);
            
            SelectedRemoteScreenResolutionIndex = indexOfNearestResolution(
                                AvailableRemoteScreenResolutions, _preferredRemoteScreenWidth);
            

            // Switch state of window, and show success
            msg = $"Successfully connected to Scripting-REST-API-Server at '{IpAddress}:{ServerPort}'";
            _logger?.LogInformation(msg);
            StatusLogLines.Add(msg);
            ConnectionEstablished = true;
            StreamStarted =false;
        }

        public async Task ButtonDisconnect_Click()
        {
            // stop eventually running streaming-session.
            if (StreamStarted)
            {
                await ButtonStopStreaming_Click();
            }

            // Switch state of window.
            string msg = "Disconnected from Scripting-REST-API-Server.";
            _logger?.LogInformation(msg);
            StatusLogLines.Add(msg);

            StreamStarted = false;
            ConnectionEstablished = false;
        }


        public async Task ButtonStartStreaming_Click()
        {
            // Change local screen-resoltution
            string res;
            res = AvailableLocalScreenResolutions[SelectedLocalScreenResolutionIndex];
            try
            {
                _wdClientServices.SetLocalScreenResolution( res );
                StatusLogLines.Add($"Successfully set screen-resolution of local computer to {res}");
            }
            catch (WDException e)
            {
                string msg=$"Couldn't set screen-resolution of local computer to {res}. Inner error-message: {e.Message}";
                _logger?.LogWarning(msg);
                StatusLogLines.Add(msg);
                // Don't bail out here, this is not too critical.
            }

            // Change remote screen-resolution
            res = AvailableRemoteScreenResolutions[SelectedRemoteScreenResolutionIndex];
            try
            {
                await _wdClientServices.SetRemoteScreenResolution( res );          
                StatusLogLines.Add($"Successfully set screen-resolution of remote computer to {res}");
            }
            catch(WDException e)
            {
                string msg = $"WARNING: Couldn't set screen-resolution of remote computer to {res}. Inner error-message: {e.Message}";
                _logger?.LogWarning(msg);
                StatusLogLines.Add(msg);
                // Don't bail out here, this is not too critical.
            }

            // Start remote streaming-sink
            string streamType;
            streamType = StreamingTypes[SelectedStreamingTypeIndex];
            try
            {
                await _wdClientServices.StartRemoteStreamingSink(streamType, StreamingPort);
                StatusLogLines.Add($"Successfully started remote streaming-sink: {streamType}, listening on port {StreamingPort}");
            }
            catch (WDException e)
            {
                string msg = $"ERROR: Couldn't start remote streaming-sink. Inner error-message: {e.Message}";
                _logger?.LogError(msg);
                StatusLogLines.Add(msg);
                // Bail out, this is critical
                return;
            }

            // Start remote prevent-screensaver-script
            try
            {
                await _wdClientServices.StartRemotePreventScreensaver();
                StatusLogLines.Add("Successfully started remote script to prevent screensaver from activating.");
            }
            catch (WDException e)
            {
                string msg = $"Couldn't Start remote script to prevent screensaver from activating. Inner error-message: {e.Message}";
                _logger?.LogWarning(msg);
                StatusLogLines.Add(msg);
                // Don't bail out here, this is not too critical.
            }

            // Start local streaming-source
            streamType = StreamingTypes[SelectedStreamingTypeIndex];
            string screenRsolutionForStreaming = 
                    AvailableRemoteScreenResolutions[SelectedRemoteScreenResolutionIndex];
  
            try
            {
                _wdClientServices.StartLocalStreamingSource( streamType, 
                                IpAddress, StreamingPort, screenRsolutionForStreaming);
                StatusLogLines.Add($"Sucessfully started local streaming-source: {streamType} to {IpAddress}:{StreamingPort} using a stream-resolution of {screenRsolutionForStreaming}");
            }
            catch (WDException e)
            {
                string msg = $"ERROR: Couldn't start local streaming-source. Inner error-message: {e.Message}";
                _logger?.LogError(msg);
                StatusLogLines.Add(msg);
                // Bail out, this is critical
                return;
            }

            // Preselect initial screen-resolutions
            SelectedLocalScreenResolutionIndex = indexOfResolution(
                    AvailableLocalScreenResolutions, InitialLocalScreenResolution );

            SelectedRemoteScreenResolutionIndex = indexOfResolution(
                    AvailableRemoteScreenResolutions, InitialRemoteScreenResolution );

            // Switch Window state
            ConnectionEstablished = true;
            StreamStarted = true;
        }

        public async Task ButtonStopStreaming_Click()
        {
            // Stop local straming-source
            try
            {
                _wdClientServices.StopLocalStreamingSource();
                StatusLogLines.Add("Sucessfully stopped local streaming-source");
            }
            catch (WDException e)
            {
                string msg = $"ERROR: Couldn't stop local streaming-source. Inner error-message: {e.Message}";
                _logger?.LogError(msg);
                StatusLogLines.Add(msg);
            }

            // Stop remote prevent-screensaver-script
            try
            {
                await _wdClientServices.StopRemotePreventScreensaver();
                StatusLogLines.Add("Successfully stopped remote script to prevent screensaver from activating.");
            }
            catch (WDException e)
            {
                string msg = $"Couldn't stop remote script, that prevents screensaver from activating. Inner error-message: {e.Message}";
                _logger?.LogWarning(msg);
                StatusLogLines.Add(msg);
            }

            // Stop remote streaming-sink
            try
            {
                await _wdClientServices.StopRemoteStreamingSink();
                StatusLogLines.Add("Sucessfully stopped remote streaming-sink");
            }
            catch (WDException e)
            {
                string msg = $"ERROR: Couldn't stop remote streaming-sink. Inner error-message: {e.Message}";
                _logger?.LogError(msg);
                StatusLogLines.Add(msg);
            }

            // Change remote screen-resolution
            string res;
            res = AvailableRemoteScreenResolutions[SelectedRemoteScreenResolutionIndex];
            try
            {
                await _wdClientServices.SetRemoteScreenResolution( res );     
                StatusLogLines.Add($"Successfully set screen-resolution of remote computer to {res}");
            }
            catch(WDException e)
            {
                string msg = $"WARNING: Couldn't set screen-resolution of remote computer to {res}. Inner error-message: {e.Message}";
                _logger?.LogWarning(msg);
                StatusLogLines.Add(msg);
            }

            // Change local screen-resoltution
            res = AvailableLocalScreenResolutions[SelectedLocalScreenResolutionIndex];
            try
            {
                _wdClientServices.SetLocalScreenResolution( res );
                StatusLogLines.Add($"Successfully set screen-resolution of local computer to {res}");
            }
            catch (WDException e)
            {
                string msg=$"Couldn't set screen-resolution of local computer to {res}. Inner error-message: {e.Message}";
                _logger?.LogWarning(msg);
                StatusLogLines.Add(msg);
                // Don't bail out here, this is not too critical.
            }

            // Pre-select preferred screen-resolutions for streaming
            SelectedLocalScreenResolutionIndex = indexOfNearestResolution(
                                AvailableLocalScreenResolutions, _preferredLocalScreenWidth);
            
            SelectedRemoteScreenResolutionIndex = indexOfNearestResolution(
                                AvailableRemoteScreenResolutions, _preferredRemoteScreenWidth);

            // Switch Window state
            ConnectionEstablished = false;
            StreamStarted = true;
 
        }

        public async Task OnWindowClose()
        {
            if (ConnectionEstablished)
            {
                await ButtonDisconnect_Click();
            }

        }


        #endregion


        //#####################################################################
        // Helper methods
        //#####################################################################
        #region

        /// <summary>
        /// Searches all provided screen-resolutions, and returns the index of 
        /// the one, whose width is nearest to desiredWidth. 
        /// </summary>
        /// <param name="resolutions">
        /// The list of screen-resolutions to search.
        /// </param>
        /// <param name="desiredWidth">
        /// The screen-resolution to find, or the nearest available one.</param>
        /// <returns>The index of the found resolution.</returns>
        private int indexOfNearestResolution( IEnumerable<string> resolutions,
                                              int desiredWidth )
        {
            int index = 0;
            int indexToSelect=-1; // worst-case: select none
            int smallestDeviation = Int32.MaxValue;

            foreach (string res in resolutions)
            {
                int width = Convert.ToInt32(res.Split('x')[0]);
                if ( Math.Abs(width-desiredWidth) <= smallestDeviation)
                {
                    smallestDeviation = Math.Abs(width-desiredWidth);
                    indexToSelect = index;
                }
                index++;
            }

            return indexToSelect;
        }


        /// <summary>
        /// Searches all provided screen-resolutions and returns the index
        /// the one that is given by resolutionToSelect.
        /// </summary>
        /// <param name="resolutions">
        /// The list of screen-resolutions to search.
        /// </param>
        /// <param name="resolutionToSelect">
        /// The screen-resolution to find
        /// </param>
        /// <returns>
        /// The index of the found resolution. If resolutionToSelect was
        /// not found, -1 is returned.
        /// </returns>
        private int indexOfResolution(IEnumerable<string> resolutions,
                                       string resolutionToSelect)
        {
            int index = 0;

            foreach (string res in resolutions)
            {
                if (res == resolutionToSelect)
                {
                    return index;
                }
                index++;
            }

            return -1; // not found
        }


        #endregion



    }
}
