using System;
using System.IO;
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
                UInt16 preferredServerPort;
                UInt16 preferredStreamingPort;
                int preferredLocalScreenWidth;
                int preferredRemoteScreenWidth;
                int indexOfpreferredStreamingType;

                realizeDependencyInjection(
                            out loggerforMainWindowModel,
                            out wdClientServices,
                            out preferredServerPort,
                            out preferredStreamingPort,
                            out preferredLocalScreenWidth,
                            out preferredRemoteScreenWidth,
                            out indexOfpreferredStreamingType);

                var mwm = new MainWindowViewModel(loggerforMainWindowModel, wdClientServices,
                            preferredServerPort, preferredStreamingPort, 
                            preferredLocalScreenWidth, preferredRemoteScreenWidth, 
                            indexOfpreferredStreamingType);

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
                    out UInt16 preferredServerPort,
                    out UInt16 preferredStreamingPort,
                    out int preferredLocalScreenWidth,
                    out int preferredRemoteScreenWidth,
                    out int indexOfpreferredStreamingType )
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
            preferredServerPort = Convert.ToUInt16(
                customConfigProvider[MagicStrings.PREFERRED_SERVER_PORT]);

            preferredStreamingPort = Convert.ToUInt16(
                customConfigProvider[MagicStrings.PREFERRED_STREAMING_PORT]);
            
            preferredLocalScreenWidth = Convert.ToInt32(
                customConfigProvider[MagicStrings.PREFERRED_LOCAL_SCREEN_WIDTH]);

            preferredRemoteScreenWidth = Convert.ToInt32(
                customConfigProvider[MagicStrings.PREFERRED_REMOTE_SCREEN_WIDTH]);
            
            indexOfpreferredStreamingType = Convert.ToInt32(
                customConfigProvider[MagicStrings.INDEX_OF_PREFERRED_STREAMING_TYPE]);


            var loggerForLocalScriptRunner = loggerFactory.CreateLogger<LocalScriptRunner>();
            var scriptDir = new DirectoryInfo(
                            customConfigProvider[MagicStrings.SCRIPT_DIRECTORY]);
            var localScriptRunner = new LocalScriptRunner(
                        loggerForLocalScriptRunner,
                        customConfigProvider[MagicStrings.SHELL],
                        customConfigProvider[MagicStrings.SHELL_ARGS_TEMPLATE],
                        scriptDir,
                        customConfigProvider[MagicStrings.SCRIPT_FILE_EXTENSION]
            );

            var loggerForRemoteScriptRunner = loggerFactory.CreateLogger<RemoteScriptRunner>();
            var remoteScriptRunner = new RemoteScriptRunner(loggerForRemoteScriptRunner);      

            var loggerForWdClientServices = loggerFactory.CreateLogger<WDClientServices>();
            wdClientServices = new WDClientServices(
                        logger : loggerForWdClientServices,
                        localScriptRunner : localScriptRunner,
                        remoteScriptRunner : remoteScriptRunner,
                        scriptNameManageScreenResolutions : customConfigProvider[MagicStrings.SCRIPTNAME_MANAGE_SCREEN_RESOLUTION],
                        scriptArgsManageScreenResolutions : customConfigProvider[MagicStrings.SCRIPTARGS_MANAGE_SCREEN_RESOLUTION],
                        scriptNameStartStreamingSink : customConfigProvider[MagicStrings.SCRIPTNAME_START_STREAMING_SINK],
                        scriptArgsStartStreamingSink : customConfigProvider[MagicStrings.SCRIPTARGS_START_STREAMING_SINK],
                        scriptNameStartStreamingSource : customConfigProvider[MagicStrings.SCRIPTNAME_START_STREAMING_SOURCE],
                        scriptArgsStartStreamingSource : customConfigProvider[MagicStrings.SCRIPTARGS_START_STREAMING_SOURCE],
                        scriptNamePreventScreensaver : customConfigProvider[MagicStrings.SCRIPTNAME_PREVENT_SCREENSAVER],
                        scriptArgsPreventScreensaver : customConfigProvider[MagicStrings.SCRIPTARGS_PREVENT_SCREENSAVER]
            );

        }


    }
}