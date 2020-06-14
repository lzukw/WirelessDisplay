using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WirelessDisplayServer.ViewModels;
using WirelessDisplayServer.Views;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WirelessDisplayServer.Services;
using WirelessDisplay.Common;


namespace WirelessDisplayServer
{
    public class App : Application
    {
//############# Original code from template - start ####################
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
//############ Original code from template - end ########################

                // Create the MainWindowModel instance
                ILogger<MainWindowViewModel> loggerForMainWindowModel;
                IServerController serverController;
                string hostName;
                string iPAddress;
                List<UInt16> portNumbers;

                realizeDependencyInjection( out loggerForMainWindowModel, 
                                            out serverController, out hostName, 
                                            out iPAddress, out portNumbers);

                MainWindowViewModel mwm = new MainWindowViewModel(loggerForMainWindowModel, 
                                            serverController, hostName, iPAddress, 
                                            portNumbers);

//############ (Nearly) original code from template - start ##################
                desktop.MainWindow = new MainWindow
                {
                    // inject MainwindowModel into the DataContext-Property of the
                    // MainWindow-view
                    DataContext = mwm
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
//############ Original code from template - end ##############################

        /// <summary>
        /// Creates all objects that need to be injected into the constructor
        /// of MainViewModel (prepare Dependency-injection). These created
        /// object themselves need other things injected in their constructors.
        /// This funciton glues all the needed objects together using 
        /// dependecy-injection.
        /// </summary>
        private void realizeDependencyInjection (
                    out ILogger<MainWindowViewModel> loggerforMainWindowModel,
                    out IServerController serverController,
                    out string hostName,
                    out string iPAddress,
                    out List<UInt16> portNumbers )
        {
            // Create LoggerFactory for typed loggers.
            // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#non-host-console-app
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Default", LogLevel.Information)
                    .AddConsole((opts)=> {opts.DisableColors=true;} );
            });

            // Assign out-object logger
            loggerforMainWindowModel = loggerFactory.CreateLogger<MainWindowViewModel>();

            // Create reader for config.json
            var customConfigProviderLogger = loggerFactory.CreateLogger<CustomConfigProvider>();
            var customConfigProvider = new CustomConfigProvider( customConfigProviderLogger,
                                                                 MAGICSTRINGS.CONFIG_FILE );

            // Read Port-Numbers from config.json
            List<UInt16> portNumberList = new List<UInt16>();

            string portsAsString = customConfigProvider[MAGICSTRINGS.PORT_NUMBERS];
            string[] ports = portsAsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (string port in ports)
            {
                UInt16 portNumber;
                bool success = UInt16.TryParse(port, out portNumber);
                if ( ! success )
                {
                    throw new WDFatalException($"In config-file '{MAGICSTRINGS.CONFIG_FILE}' the port-numbers cannot be parsed: '{MAGICSTRINGS.PORT_NUMBERS} : {portsAsString}'");
                }
                portNumberList.Add(portNumber);
            }
            // Assign out-object portNumbers
            portNumbers = portNumberList;

            // Create instance implementing IServerController
            var serverControllerLogger = loggerFactory.CreateLogger<ServerController>();
            string serverPath = 
                customConfigProvider[MAGICSTRINGS.PATH_TO_SCRIPTING_REST_API_SERVER];
            string argsTemplate = 
                customConfigProvider[MAGICSTRINGS.ARGS_TEMPLATE_FOR_SCRIPTING_REST_API_SERVER];
            // Assign out-object serverController
            serverController = new ServerController(serverControllerLogger,
                                                    serverPath, argsTemplate);
            

            // Assign out-object hostName
            hostName = HostAddressProvider.HostName;

            // Assign out-object iPAddress
            iPAddress = HostAddressProvider.IPv4Address;
        }


    }
}