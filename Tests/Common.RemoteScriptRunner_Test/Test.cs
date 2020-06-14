using NUnit.Framework;
using WirelessDisplay.Common;

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace WirelessDisplay.Tests.Common.RemoteScriptRunner_Test
{

    [TestFixture]
    public class Test
    {
        public const string SERVER_IP = "127.0.0.1";
        public const string SERVER_PORT = "6789";

        // NOTE: If using dotnet run (and not dotnet test), remove the first 
        //three ../../../ from path.
        public const string SERVER_EXECUTABLE_PATH_LINUX = @"../../../../../ScriptingRestApiServer_executable/ScriptingRestApiServer";
        public const string SERVER_CMD_LINE_ARGS = "--urls=http://" + SERVER_IP + ":" +SERVER_PORT;
        public const string TEST_SCRIPT_FOR_RUN = "testscript_for_run";
        public const string TEST_SCRIPT_FOR_START_STOP = "testscript_for_start_stop";
        public static readonly string[] TEST_SCRIPT_ARGS = new string[] { "line1", "line2", "line3", "line4" };
        public const string TEST_SCRIPT_STDIN = "from stdin\n";
        public const int TEST_SCRIPT_EXITCODE = 5;
        
        private Process _serverProcess;
        private IRemoteScriptRunner _scriptRunner;

        [SetUp]
        public void Setup()
        {
            // Start Scripting-REST-API-Server
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
 
            // Logging:
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Default", LogLevel.Debug).AddConsole();
            });
            var logger = loggerFactory.CreateLogger<RemoteScriptRunner>();

            _scriptRunner = new RemoteScriptRunner(logger);
            _scriptRunner.SetIpAddressAndPort( SERVER_IP, Convert.ToUInt16(SERVER_PORT));
        }

        [TearDown]
        public void TearDown()
        {
            _serverProcess.Kill( entireProcessTree : true );
        }



        [Test]
        public async Task RunAndWaitForScript_NormalUsecase_everythingOk()
        {
            (int exitCode, List<string> stdoutLines, List<string> stderrLines) = await
                        _scriptRunner.RunAndWaitForScript(
                            scriptName : TEST_SCRIPT_FOR_RUN, 
                            scriptArgs : string.Join(" ", TEST_SCRIPT_ARGS), 
                            stdin :      TEST_SCRIPT_STDIN
                        );

            Assert.True( exitCode == TEST_SCRIPT_EXITCODE );
            
            // The testscript writes the first two arguments and the line read from stin to stdout
            Assert.AreEqual( stdoutLines[0], TEST_SCRIPT_ARGS[0]);
            Assert.AreEqual( stdoutLines[1], TEST_SCRIPT_ARGS[1]);
            Assert.AreEqual( stdoutLines[2], TEST_SCRIPT_STDIN.Trim() );

            // The testsript writes the third and fourth argument to stderr
            Assert.AreEqual( stderrLines[0], TEST_SCRIPT_ARGS[2]);
            Assert.AreEqual( stderrLines[1], TEST_SCRIPT_ARGS[3]);            
        }

        [Test]
        public void RunAndWaitForScript_ScriptNotStopping_ThrowsWDException()
        {
            // Not the not ending script is started.
            
            
            Assert.ThrowsAsync<WDException>( async ()=>
            {
                (int exitCode, List<string> stdoutLines, List<string> stderrLines) = await
                _scriptRunner.RunAndWaitForScript( 
                            scriptName : TEST_SCRIPT_FOR_START_STOP, 
                            scriptArgs : string.Join(" ", TEST_SCRIPT_ARGS), 
                            stdin :      TEST_SCRIPT_STDIN
                            );
            } );
        }


        [Test]
        public async Task StartQueryAndStopScript_NormalUsecase_everythingOk()
        {
                        // Start script
            int processId = await _scriptRunner.StartScript( 
                            scriptName : TEST_SCRIPT_FOR_START_STOP, 
                            scriptArgs : string.Join(" ", TEST_SCRIPT_ARGS), 
                            stdin :      TEST_SCRIPT_STDIN
                            );

            Assert.NotZero(processId);

            System.Diagnostics.Debug.Write("Waiting 11 seconds");
            for (int i=0; i<11; i++)
            {
                System.Threading.Thread.Sleep(1000);
                System.Diagnostics.Debug.Write(".");
            }

            // Query, if script is still running
            bool isRunning = await _scriptRunner.IsScriptRunning(processId);
            Assert.True(isRunning);

            // Stop script
            (int exitCode, List<string> stdoutLines, List<string> stderrLines) = await
                            _scriptRunner.StopScript(processId);
            
            // Exit code is not 5, because "sleep"-command is killed and produces an
            // exit-codes like 137. 
            Assert.NotZero( exitCode );
            
            // The testscript writes the first two arguments and the line read from stin to stdout
            Assert.AreEqual( stdoutLines[0], TEST_SCRIPT_ARGS[0]);
            Assert.AreEqual( stdoutLines[1], TEST_SCRIPT_ARGS[1]);
            Assert.AreEqual( stdoutLines[2], TEST_SCRIPT_STDIN.Trim() );

            // The testsript writes the third and fourth argument to stderr
            Assert.AreEqual( stderrLines[0], TEST_SCRIPT_ARGS[2]);
            Assert.AreEqual( stderrLines[1], TEST_SCRIPT_ARGS[3]);
        }

        [Test]
        public void Startscript_ScriptExitsEarly_ThrowsWDException()
        {
            Assert.ThrowsAsync<WDException>( async () =>
            {
                int processId = await _scriptRunner.StartScript( 
                                scriptName : TEST_SCRIPT_FOR_RUN, 
                                scriptArgs : string.Join(" ", TEST_SCRIPT_ARGS), 
                                stdin :      TEST_SCRIPT_STDIN
                                );
            } );
        }




        // The own Main-Method was added, to be able to debug in Visual-Studio-Code
        // (Normally the tests habe to be run with dotnet test, which is not done, but
        // pressing F5 in Visual-Studio-Code does a dotnet run).
        //
        // Now there are two Main()-Methods, which causes a compiler-error. To fix it
        // the following line was added to Common.Tests.csproj in the section <PropertyGroup>:
        // <StartupObject>WirelessDisplay.UnitTests.Common.Tests.ScriptRunner_Test</StartupObject>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Stopping Sripting-REST-API-Server.");
            var test = new Test();
            test.Setup();

            Console.WriteLine("Starting Tests");
            await test.RunAndWaitForScript_NormalUsecase_everythingOk();
            test.RunAndWaitForScript_ScriptNotStopping_ThrowsWDException();
            await test.StartQueryAndStopScript_NormalUsecase_everythingOk();
            test.Startscript_ScriptExitsEarly_ThrowsWDException();

            Console.WriteLine("All tests passed. Stopping Sripting-REST-API-Server.");
            test.TearDown();
        }


    }

}