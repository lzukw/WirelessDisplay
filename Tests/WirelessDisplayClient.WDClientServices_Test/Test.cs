using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WirelessDisplay.Common;
using WirelessDisplayClient.Services;

namespace WirelessDisplay.Tests.WirelessDisplayClient.WDClientServices_Test
{
    public class Test
    {
        const string SERVER_IP = "127.0.0.1";
        const string SERVER_PORT = "6789";

        const string SERVER_EXECUTABLE_PATH_LINUX = @"../../../../../ScriptingRestApiServer_executable/ScriptingRestApiServer";
        const string SERVER_CMD_LINE_ARGS = "--urls=http://" + SERVER_IP + ":" + SERVER_PORT;

        const string SCRIPT_DIR_LINUX = "../../../../../Scripts/Linux";
        const string SHELL_LINUX = "bash";
        const string SHELL_ARGS_LINUX = "{SCRIPT} {SCRIPT_ARGS}";
        const string SCRIPT_FILE_EXTENSION_LINUX = ".sh";
        const string SCRIPT_NAME_MANAGE_SCREEN_RESOLUTIONS = "manageScreenResolutions";
        const string SCRIPT_ARGS_MANAGE_SCREEN_RESOLUTIONS = "{ACTION} {SCREEN_RESOLUTION}";
        const string SCRIPT_NAME_START_STREAMING_SINK = "startStreamingSink";
        const string SCRIPT_ARGS_START_STREAMING_SINK = "{STREAMING_TYPE} {PORT}";
        const string SCRIPT_NAME_START_STREAMING_SOURCE = "startStreamingSource";
        const string SCRIPT_ARGS_START_STREAMING_SOURCE = "{STREAMING_TYPE} {IP} {PORT} {SCREEN_RESOLUTION}";
        const string SCRIPT_NAME_PREVENT_SCREENSAVER = "preventScreensaver";
        const string SCRIPT_ARGS_PREVENT_SCREENSAVER = "{SECONDS}";
        const string ARG_FOR_STREAMING_TYPE = "VNC";
        const UInt16 ARG_FOR_STREAMING_PORT = 5500;
        const string ARG_FOR_SCREEN_RESOLUTION_STREAM = "1024x768";


        private IWDClientServices _wdClientServices;
        private Process _serverProcess;


        private void startScriptingRestApiServer()
        {
            var serverExecutable = new FileInfo(SERVER_EXECUTABLE_PATH_LINUX);
            Assert.True( serverExecutable.Exists );
            _serverProcess = new Process();
            _serverProcess.StartInfo.FileName = serverExecutable.FullName;
            _serverProcess.StartInfo.Arguments = SERVER_CMD_LINE_ARGS;
            _serverProcess.StartInfo.WorkingDirectory = serverExecutable.Directory.FullName;

            _serverProcess.Start();

            //wait until server starts
            Console.Write("Waiting 5 Seconds for REST-API-Server to start");
            for (int i=0; i<5; i++)
            {
                Thread.Sleep(1000);
                Console.Write(".");
            }
            Console.WriteLine();
        }

        private void stopScriptingRestApiServer()
        {
            _serverProcess.Kill( entireProcessTree : true );
        }

        private void createWdClientServices()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Default", LogLevel.Information)
                    .AddConsole();
            });
            
            // Create LocalScriptRunner.
            var loggerForLocalScriptRunner = loggerFactory.CreateLogger<LocalScriptRunner>();
            var scriptDir = new DirectoryInfo(SCRIPT_DIR_LINUX);
            var localScriptRunner = new LocalScriptRunner(
                        loggerForLocalScriptRunner,
                        SHELL_LINUX,
                        SHELL_ARGS_LINUX,
                        scriptDir,
                        SCRIPT_FILE_EXTENSION_LINUX
            );

            // Create RemoteScriptRunner.
            var loggerForRemoteScriptRunner = loggerFactory.CreateLogger<RemoteScriptRunner>();
            var remoteScriptRunner = new RemoteScriptRunner(loggerForRemoteScriptRunner); 

            // Create WDClientServices, passing it the LocalScriptRunner and 
            // the RemoteScriptRunner.
            var loggerForWdClientServices = loggerFactory.CreateLogger<WDClientServices>();
            _wdClientServices = new WDClientServices(
                        logger : loggerForWdClientServices,
                        localScriptRunner : localScriptRunner,
                        remoteScriptRunner : remoteScriptRunner,
                        scriptNameManageScreenResolutions : SCRIPT_NAME_MANAGE_SCREEN_RESOLUTIONS,
                        scriptArgsManageScreenResolutions : SCRIPT_ARGS_MANAGE_SCREEN_RESOLUTIONS,
                        scriptNameStartStreamingSink : SCRIPT_NAME_START_STREAMING_SINK,
                        scriptArgsStartStreamingSink : SCRIPT_ARGS_START_STREAMING_SINK,
                        scriptNameStartStreamingSource : SCRIPT_NAME_START_STREAMING_SOURCE,
                        scriptArgsStartStreamingSource : SCRIPT_ARGS_START_STREAMING_SOURCE,
                        scriptNamePreventScreensaver : SCRIPT_NAME_PREVENT_SCREENSAVER,
                        scriptArgsPreventScreensaver : SCRIPT_ARGS_PREVENT_SCREENSAVER 
            );

        }


        [SetUp]
        public void Setup()
        {
            createWdClientServices();
            startScriptingRestApiServer();
        }


        [TearDown]
        public void TearDown()
        {
            stopScriptingRestApiServer();
        }


        [Test]
        public void ManageLocalScreenResolutions_NormalUseCase_ShouldPass()
        {
            Console.WriteLine("Testing managing of screen-resolutions on local computer:");
            Console.WriteLine("=========================================================");
            
            string initialLocalScreenResolution = 
                _wdClientServices.GetInitalLocalScreenResolution();
            Assert.False (string.IsNullOrEmpty(initialLocalScreenResolution));
            Console.WriteLine($"Initial local screen-resolution is: {initialLocalScreenResolution}");

            List<string> availableLocalScreenResolutions =
                _wdClientServices.GetAvailableLocalScreenResolutions();
            Assert.True(availableLocalScreenResolutions.Count > 0);
            Console.WriteLine("Available local sreen-resolutions:");
            foreach (string res in availableLocalScreenResolutions)
            {
                Console.WriteLine($"   {res}");
            }

            Console.WriteLine("Switching to second available local screen-resolution.");
            Assert.DoesNotThrow( ()=>
            {
                _wdClientServices.SetLocalScreenResolution(
                            availableLocalScreenResolutions[1]);
            });
            Thread.Sleep(2000);

            Console.WriteLine("Getting current local screen-resolution:");
            string currentLocalScreenResoltuion = 
                _wdClientServices.GetCurrentLocalScreenResolution();
            Assert.False (string.IsNullOrEmpty(currentLocalScreenResoltuion));
            Console.WriteLine($"Current local screen-resolution is: {currentLocalScreenResoltuion}");

            Console.WriteLine("Switching back to initial local screen-resolution.");
            Assert.DoesNotThrow( ()=>
            {
                _wdClientServices.SetLocalScreenResolution(
                            initialLocalScreenResolution);
            });
            
        }

        [Test]
        public async Task ConnectAndRemoteScreenResolutions_NormalUseCase_ShouldPass()
        {
            Console.WriteLine("");
            Console.WriteLine("Connecting to remote scripting-REST-API-Server");
            try
            {
                await _wdClientServices.ConnectToServer(SERVER_IP, 
                            Convert.ToUInt16(SERVER_PORT));
            }
            catch(WDException)
            {
                Assert.Fail();
            }

            Console.WriteLine("Testing managing of screen-resolutions on remote computer:");
            Console.WriteLine("=========================================================");
            
            string initialRemoteScreenResolution = await
                _wdClientServices.GetInitalRemoteScreenResolution();
            Assert.False (string.IsNullOrEmpty(initialRemoteScreenResolution));
            Console.WriteLine($"Initial remote screen-resolution is: {initialRemoteScreenResolution}");

            List<string> availableRemoteScreenResolutions = await
                _wdClientServices.GetAvailableRemoteScreenResolutions();
            Assert.True(availableRemoteScreenResolutions.Count > 0);
            Console.WriteLine("Available remote sreen-resolutions:");
            foreach (string res in availableRemoteScreenResolutions)
            {
                Console.WriteLine($"   {res}");
            }

            Console.WriteLine("Switching to second available remote screen-resolution.");
            try
            {
                await _wdClientServices.SetRemoteScreenResolution(
                            availableRemoteScreenResolutions[1]);
            }
            catch(Exception)
            {
                Assert.Fail();
            }
            Thread.Sleep(2000);

            Console.WriteLine("Getting current remote screen-resolution:");
            string currentRemoteScreenResoltuion = await
                _wdClientServices.GetCurrentRemoteScreenResolution();
            Assert.False (string.IsNullOrEmpty(currentRemoteScreenResoltuion));
            Console.WriteLine($"Current remote screen-resolution is: {currentRemoteScreenResoltuion}");

            Console.WriteLine("Switching back to initial remote screen-resolution.");
            try
            {
                await _wdClientServices.SetRemoteScreenResolution(
                            initialRemoteScreenResolution);
            }
            catch(Exception)
            {
                Assert.Fail();
            }

        }

        [Test]
        public async Task StartAndStopStreaming_NormalUseCase_ShouldPass()
        {
            Console.WriteLine("Test streaming:");
            Console.WriteLine("===============");
            Console.WriteLine("Connecting to remote scripting-REST-API-Server");
            try
            {
                await _wdClientServices.ConnectToServer(SERVER_IP, 
                            Convert.ToUInt16(SERVER_PORT));
            }
            catch(WDException)
            {
                Assert.Fail();
            }

            Console.WriteLine("Starting remote streaming sink");
            try
            {
                await _wdClientServices.StartRemoteStreamingSink(ARG_FOR_STREAMING_TYPE,
                        ARG_FOR_STREAMING_PORT);
            }
            catch (WDException)
            {
                Assert.Fail();
            }

            Console.WriteLine("Starting local Streaming source.");
            Assert.DoesNotThrow( ()=>
            {
                _wdClientServices.StartLocalStreamingSource( ARG_FOR_STREAMING_TYPE, 
                                SERVER_IP, ARG_FOR_STREAMING_PORT , ARG_FOR_SCREEN_RESOLUTION_STREAM);
            });

            Thread.Sleep(10000);

            Console.WriteLine("Stopping local Streaming source.");
            Assert.DoesNotThrow( ()=>
            {
                _wdClientServices.StopLocalStreamingSource();
            });

            Console.WriteLine("Stopping remote streaming sink.");
            try
            {
                await _wdClientServices.StopRemoteStreamingSink();
            }
            catch (WDException)
            {
                Assert.Fail();
            }
        }


        [Test]
        public async Task StartAndStopScriptToPreventScreensaver_NormalUseCase_ShouldPass()
        {
            Console.WriteLine("Test prevent-remote-screensaver-from-activating:");
            Console.WriteLine("================================================");
            Console.WriteLine("Connecting to remote scripting-REST-API-Server");
            try
            {
                await _wdClientServices.ConnectToServer(SERVER_IP, 
                            Convert.ToUInt16(SERVER_PORT));
            }
            catch(WDException)
            {
                Assert.Fail();
            }

            Console.WriteLine("Starting remote script to prevent screensaver from activating");
            try
            {
                await _wdClientServices.StartRemotePreventScreensaver();
            }
            catch (WDException)
            {
                Assert.Fail();
            }

            Console.Write ("Waiting for 20 seconds:");
            for (int i = 0; i<20; i++)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine();

            Console.WriteLine("Stopping remote script to prevent screensaver from activating");
            try
            {
                await _wdClientServices.StopRemotePreventScreensaver();
            }
            catch (WDException)
            {
                Assert.Fail();
            }

        }



        // The own Main-Method was added, to be able to debug in Visual-Studio-Code
        // (Normally the tests habe to be run with dotnet test, which is not done, but
        // pressing F5 in Visual-Studio-Code does a dotnet run).
        //
        // Now there are two Main()-Methods, which causes a compiler-error. To fix it
        // the following line was added to the csproj-file in the section <PropertyGroup>:
        // <StartupObject>WirelessDisplay.Tests.WirelessDisplayClient.WDClientServices_Test.Tests</StartupObject>
        public static async Task Main(string[] args)
        {
            var test = new Test();
            test.Setup();
            
            // You can individually comment out tests
            //test.ManageLocalScreenResolutions_NormalUseCase_ShouldPass();
            //await test.ConnectAndRemoteScreenResolutions_NormalUseCase_ShouldPass();
            //await test.StartAndStopStreaming_NormalUseCase_ShouldPass();
            await test.StartAndStopScriptToPreventScreensaver_NormalUseCase_ShouldPass();

            test.TearDown();
        }
    }
}