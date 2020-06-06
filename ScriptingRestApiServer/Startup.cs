using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.IO;
using System.Text.Json;
using System.Runtime.InteropServices;
using WirelessDisplay.Common;

namespace ScriptingRestApiServer
{
    public class Startup
    {
        /// <summary> Path to the configuration file specific for this project </summary>
        private const string CONFIG_FILE ="config.json";

        private const string SHELL = "Shell";
        private const string SHELL_ARGS_TEMPLATE = "ShellArgsTemplate";
        private const string SCRIPT_DIRECTORY = "ScriptDirectory";
        private const string SCRIPT_FILE_EXTENSION = "ScriptFileExtension";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Added Dependency-Injection: Here a Singleton-Instane of ScriptRunner 
            // is created. It will be created upn the first API-Reuest and then
            // passed to the constructor of the webapi-Controller, that serves the request.
            services.AddSingleton<IScriptRunner>( (s) =>
            {
                // For debugging purposes: Show current working directory, since 
                // script-paths are relative
                Console.WriteLine($"Current working-directory is: '{System.IO.Directory.GetCurrentDirectory()}'");

                var logger = s.GetRequiredService<ILogger<ScriptRunner>>();
                var loggerForCustomConfig = s.GetRequiredService<ILogger<CustomConfigProvider>>();

                CustomConfigProvider myConfig = 
                        new CustomConfigProvider(loggerForCustomConfig, CONFIG_FILE);

                DirectoryInfo scriptDir = 
                        new DirectoryInfo(myConfig.GetValue(SCRIPT_DIRECTORY));
                if ( ! scriptDir.Exists)
                {
                    string msg=$"FATAL: Cannot find Script-directory '{scriptDir.FullName}'";
                    Console.WriteLine(msg);
                    throw new WDFatalException(msg);
                }
                return new ScriptRunner (logger, 
                                         myConfig.GetValue(SHELL),
                                         myConfig.GetValue(SHELL_ARGS_TEMPLATE),
                                         scriptDir,
                                         myConfig.GetValue(SCRIPT_FILE_EXTENSION));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
