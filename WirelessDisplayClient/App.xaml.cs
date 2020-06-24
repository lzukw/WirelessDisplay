using System;
using System.IO;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using WirelessDisplayClient.ViewModels;
using WirelessDisplayClient.Views;
using WirelessDisplayClient.Services;
using WirelessDisplay.Common;
using Microsoft.Extensions.Logging;

namespace WirelessDisplayClient
{
    public class App : Application
    {
//############### Original code from template - start ###############
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
//############### Original code from template - end #################

                ILogger<MainWindowViewModel> loggerforMainWindowModel;
                IWDClientServices wdClientServices;
                string[] streamingTypes;
                UInt16 preferredServerPort;
                UInt16 preferredStreamingPort;
                int preferredLocalScreenWidth;
                int preferredRemoteScreenWidth;

                realizeDependencyInjection(
                            out loggerforMainWindowModel,
                            out wdClientServices,
                            out streamingTypes,
                            out preferredServerPort,
                            out preferredStreamingPort,
                            out preferredLocalScreenWidth,
                            out preferredRemoteScreenWidth);

                var mwm = new MainWindowViewModel(
                            logger : loggerforMainWindowModel, 
                            wdClientServices : wdClientServices,
                            streamingTypes : streamingTypes,
                            preferredServerPort :preferredServerPort, 
                            preferredStreamingPort : preferredStreamingPort, 
                            preferredLocalScreenWidth : preferredLocalScreenWidth, 
                            preferredRemoteScreenWidth : preferredRemoteScreenWidth );

//############ (Nearly) original code from template - start ################
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mwm
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
//############ Original code from template - end ############################

        /// <summary>
        /// Creates all objects that need to be injected into the constructor
        /// of MainViewModel (prepare Dependency-injection). These created
        /// object themselves need other things injected in their constructors.
        /// This funciton glues all the needed objects together using 
        /// dependecy-injection.
        /// </summary>
        private void realizeDependencyInjection(
                    out ILogger<MainWindowViewModel> loggerforMainWindowModel,
                    out IWDClientServices wdClientServices,
                    out string[] streamingTypes,
                    out UInt16 preferredServerPort,
                    out UInt16 preferredStreamingPort,
                    out int preferredLocalScreenWidth,
                    out int preferredRemoteScreenWidth )
        {
            // Create typed loggers.
            // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1#non-host-console-app
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Default", LogLevel.Information)
                    .AddConsole();
            });

            // Assign out-object logger
            loggerforMainWindowModel = loggerFactory.CreateLogger<MainWindowViewModel>();

            // Create reader for config.json
            var customConfigProviderLogger = loggerFactory.CreateLogger<CustomConfigProvider>();
            var customConfigProvider = new CustomConfigProvider( customConfigProviderLogger,
                                                                 MagicStrings.CONFIG_FILE );

            // Assign out-objects read from config.json
            string streamtypeList = customConfigProvider[MagicStrings.STREAMING_TYPES];
            streamingTypes = streamtypeList.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i<streamingTypes.Length; i++)
            {
                streamingTypes[i] = streamingTypes[i].Trim();
            }

            preferredServerPort = Convert.ToUInt16(
                customConfigProvider[MagicStrings.PREFERRED_SERVER_PORT]);

            preferredStreamingPort = Convert.ToUInt16(
                customConfigProvider[MagicStrings.PREFERRED_STREAMING_PORT]);
            
            preferredLocalScreenWidth = Convert.ToInt32(
                customConfigProvider[MagicStrings.PREFERRED_LOCAL_SCREEN_WIDTH]);

            preferredRemoteScreenWidth = Convert.ToInt32(
                customConfigProvider[MagicStrings.PREFERRED_REMOTE_SCREEN_WIDTH]);

            bool letShellWindowsPopUpWhenStartScript;
            bool success = bool.TryParse(customConfigProvider[MagicStrings.LET_SHELL_WINDOW_POP_UP_WHEN_START_SCRIPT], out letShellWindowsPopUpWhenStartScript);
            if (! success )
            {
                throw new WDFatalException($"Value '{customConfigProvider[MagicStrings.LET_SHELL_WINDOW_POP_UP_WHEN_START_SCRIPT]}' for key '{MagicStrings.LET_SHELL_WINDOW_POP_UP_WHEN_START_SCRIPT}' in configuration-file '{MagicStrings.CONFIG_FILE}' is not 'true' or 'false'.");
            }

            var loggerForLocalScriptRunner = loggerFactory.CreateLogger<LocalScriptRunner>();
            var scriptDir = new DirectoryInfo(
                            customConfigProvider[MagicStrings.SCRIPT_DIRECTORY]);
            var localScriptRunner = new LocalScriptRunner(
                        logger : loggerForLocalScriptRunner,
                        shell : customConfigProvider[MagicStrings.SHELL],
                        shellArgsTemplate : customConfigProvider[MagicStrings.SHELL_ARGS_TEMPLATE],
                        scriptDirectory : scriptDir,
                        scriptExtension : customConfigProvider[MagicStrings.SCRIPT_FILE_EXTENSION],
                        letShellWindowsPopUpWhenStartScript : letShellWindowsPopUpWhenStartScript
            );

            var loggerForRemoteScriptRunner = loggerFactory.CreateLogger<RemoteScriptRunner>();
            var remoteScriptRunner = new RemoteScriptRunner(loggerForRemoteScriptRunner);      

            string localIpAddress = HostAddressProvider.IPv4Address;

            var loggerForWdClientServices = loggerFactory.CreateLogger<WDClientServices>();
            wdClientServices = new WDClientServices(
                        logger : loggerForWdClientServices,
                        localScriptRunner : localScriptRunner,
                        localIpAddress : localIpAddress,
                        remoteScriptRunner : remoteScriptRunner,
                        scriptNameManageScreenResolutions : customConfigProvider[MagicStrings.SCRIPTNAME_MANAGE_SCREEN_RESOLUTION],
                        scriptArgsManageScreenResolutions : customConfigProvider[MagicStrings.SCRIPTARGS_MANAGE_SCREEN_RESOLUTION],
                        scriptNameStartStreamingSink : customConfigProvider[MagicStrings.SCRIPTNAME_START_STREAMING_SINK],
                        scriptArgsStartStreamingSink : customConfigProvider[MagicStrings.SCRIPTARGS_START_STREAMING_SINK],
                        scriptNameStartStreamingSource : customConfigProvider[MagicStrings.SCRIPTNAME_START_STREAMING_SOURCE],
                        scriptArgsStartStreamingSource : customConfigProvider[MagicStrings.SCRIPTARGS_START_STREAMING_SOURCE],
                        scriptNamePreventDisplayBlanking : customConfigProvider[MagicStrings.SCRIPTNAME_PREVENT_DISPLAY_BLANKING],
                        scriptArgsPreventDisplayBlanking : customConfigProvider[MagicStrings.SCRIPTARGS_PREVENT_DISPLAY_BLANKING]
            );

        }


    }
}