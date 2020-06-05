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

        // Keys in CONFIG_FILE:
        private const string LINUX = "Linux";
        private const string MAC_OS = "macOS";
        private const string WINDOWS = "Windows";
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

            // For debugging purposes: Show current working directory, since 
            // script-paths are relative
            Console.WriteLine($"Current working-directory is: '{System.IO.Directory.GetCurrentDirectory()}'");

            //
            // Then read the own configuration (CONFIG_FILE aka config.json)
            //
            Dictionary<string,Dictionary<string,string>> myConfig;
            try
            {
                string myConfigFileContents = File.ReadAllText(CONFIG_FILE);
                myConfig = JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,string>>>(myConfigFileContents);
            }
            catch (FileNotFoundException fe)
            {
                Console.WriteLine($"FATAL: Could not find Configuration-File '{CONFIG_FILE}'");
                throw fe;
            }
            catch (JsonException je)
            {
                Console.WriteLine($"FATAL: Configuration-File '{CONFIG_FILE}' could not be deserialized to 'Dictionary<string,Dictionary<string,string>>'.");
                throw je;
            }

            string myConfigSection;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                myConfigSection=LINUX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                myConfigSection=MAC_OS;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                myConfigSection=WINDOWS;
            }
            else
            {
                throw new Exception("Operating System not supported");
            }

            // Added Dependency-Injection: Here a Singleton-Instane of ScriptRunner 
            // is created. It will be created upn the first API-Reuest and then
            // passed to the constructor of the webapi-Controller, that serves the request.
            services.AddSingleton<IScriptRunner>( (s) =>
            {
                var logger = s.GetRequiredService<ILogger<ScriptRunner>>();
                DirectoryInfo scriptDir = new DirectoryInfo(myConfig[myConfigSection][SCRIPT_DIRECTORY]);
                if ( ! scriptDir.Exists)
                {
                    string msg=$"FATAL: Cannot find Script-directory '{scriptDir.FullName}'";
                    Console.WriteLine(msg);
                    throw new FileNotFoundException(msg);
                }
                return new ScriptRunner (logger, 
                                         myConfig[myConfigSection][SHELL],
                                         myConfig[myConfigSection][SHELL_ARGS_TEMPLATE],
                                         scriptDir,
                                         myConfig[myConfigSection][SCRIPT_FILE_EXTENSION]);
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
