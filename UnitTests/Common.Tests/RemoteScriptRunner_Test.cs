using NUnit.Framework;
using WirelessDisplay.Common;

using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace WirelessDisplay.UnitTests.Common.Tests
{
    [TestFixture]
    public class RemoteScriptRunner_Test
    {
        const string SERVER_IP = "127.0.0.1";
        const string SERVER_PORT = "6789";

        // NOTE: If using dotnet run (and not dotnet test), remove the first 
        //three ../../../ from path.
        const string SERVER_EXECUTABLE_PATH_LINUX = @"../../ScriptingRestApiServer_executable/ScriptingRestApiServer";
        const string SERVER_CMD_LINE_ARGS = "--url=http://" + SERVER_IP + ":" +SERVER_PORT;

        private Process _serverProcess;

        [SetUp]
        public void Setup()
        {
            // Start Scripting-REST-API-Server
            Assert.True( File.Exists(SERVER_EXECUTABLE_PATH_LINUX));
            _serverProcess = new Process();
            _serverProcess.StartInfo.FileName = SERVER_EXECUTABLE_PATH_LINUX;
            _serverProcess.StartInfo.Arguments = SERVER_CMD_LINE_ARGS;
            _serverProcess.Start();

            //wait until server starts
            Thread.Sleep(10000);

            // Logging:
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Default", LogLevel.Debug).AddConsole();
            });
            var logger = loggerFactory.CreateLogger<RemoteScriptRunner>();


        }

        [TearDown]
        public void TearDown()
        {
            _serverProcess.Kill( entireProcessTree : true );
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
            var test = new RemoteScriptRunner_Test();
            test.Setup();


            test.TearDown();
        }


    }

}