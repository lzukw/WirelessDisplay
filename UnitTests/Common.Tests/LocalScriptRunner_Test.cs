// NUnit-Testing:
// https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit
using NUnit.Framework;
using WirelessDisplay.Common;

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;


// dotnet add package Microsoft.Extensions.Logging
using Microsoft.Extensions.Logging;

// dotnet add package Microsoft.Extensions.Logging.Console
using Microsoft.Extensions.Logging.Console;

namespace WirelessDisplay.UnitTests.Common.Tests
{
    [TestFixture]
    public class LocalScriptRunner_Test
    {
        // The magic strings needed for running scripts (only Linux)
        const string SHELL_LINUX = "bash";
        const string SHELL_ARGS_TEMPLATE_LINUX="-c \"{SCRIPT} {SCRIPT_ARGS}\"";

        // NOTE: If using dotnet run (and not dotnet test), remove the first 
        //three ../../../ from path.
        const string SCRIPT_DIR_LINUX = @"../../../../../Scripts/Linux";
        const string SCRIPT_EXTENSION_LINUX = ".sh";

        const string TEST_SCRIPT_FOR_RUN = "testscript_for_run";
        const string TEST_SCRIPT_FOR_START_STOP = "testscript_for_start_stop";
        readonly string[] TEST_SCRIPT_ARGS = new string[] { "line1", "line2", "line3", "line4" };
        const string TEST_SCRIPT_STDIN = "from stdin\n";
        const int TEST_SCRIPT_EXITCODE = 5;

        ILocalScriptRunner _scriptRunner;


        [SetUp]
        public void Setup()
        {
            // Logging:
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Default", LogLevel.Debug).AddConsole();
            });
            var logger = loggerFactory.CreateLogger<LocalScriptRunner>();

            // instantiate an (I)ScriptRunner
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                DirectoryInfo scriptDir = new DirectoryInfo( SCRIPT_DIR_LINUX );
                _scriptRunner = new LocalScriptRunner(logger, SHELL_LINUX,SHELL_ARGS_TEMPLATE_LINUX, scriptDir, SCRIPT_EXTENSION_LINUX);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException("No test for macOS yet available");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotImplementedException("No test for Windows yet available");
            }
        }

        [Test]
        public void RunAndWaitForScript_NormalUsecase_everythingOk()
        {

            (int exitCode, List<string> stdoutLines, List<string> stderrLines) = _scriptRunner.RunAndWaitForScript( 
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
            
            
            Assert.Throws<WDException>( ()=>
            {
                (int exitCode, List<string> stdoutLines, List<string> stderrLines) = _scriptRunner.RunAndWaitForScript( 
                            scriptName : TEST_SCRIPT_FOR_START_STOP, 
                            scriptArgs : string.Join(" ", TEST_SCRIPT_ARGS), 
                            stdin :      TEST_SCRIPT_STDIN
                            );
            } );
        }

        [Test]
        public void StartScript_NormalUse_everythingOk()
        {
            int processId = _scriptRunner.StartScript( 
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
            Assert.True( _scriptRunner.IsScriptRunning(processId) );

            (int exitCode, List<string> stdoutLines, List<string> stderrLines) = 
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
            Assert.Throws<WDException>( () =>
            {
                int processId = _scriptRunner.StartScript( 
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
        public static void Main(string[] args)
        {
            var test = new LocalScriptRunner_Test();
            test.Setup();

            test.RunAndWaitForScript_NormalUsecase_everythingOk();
            test.RunAndWaitForScript_ScriptNotStopping_ThrowsWDException();

            test.StartScript_NormalUse_everythingOk();
            test.Startscript_ScriptExitsEarly_ThrowsWDException();
        }

    }
}
