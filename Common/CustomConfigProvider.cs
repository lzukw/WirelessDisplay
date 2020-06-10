using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace WirelessDisplay.Common
{
    /// <summary>
    /// Used to read key-value-pairs from a custom json-configuration-file.
    /// This json-File contains one josn-object containing the properties (sections)
    /// "PlatformIndependent", "Linux", "macOs" and "Windows". Each section
    /// contains key-value-pairs, where the keys and values are strings.
    /// </summary>
    public class CustomConfigProvider
    {


        /// <summary> The path to the json-config-file. </summary>
        private FileInfo _jsonConfigFile;

        /// <summary> The deserialized contents of the json-config-file. </summary>
        private Dictionary<string,Dictionary<string,string>> _jsonConfig;
        
        /// <summary> 
        /// Either "Linux", "macOS" or "Windows", depending on the
        /// current operating-system.
        /// </summary>
        private readonly string _osSection;

        /// <summary> The logger. </summary>
        private ILogger<CustomConfigProvider> _logger;


        /// <summary> Constructor. </summary>
        /// <param name="logger">Used for logging.</param>
        /// <param name="pathToJsonConfigFile">
        ///     File-path to the json-configuration-file.
        /// </param>
        public CustomConfigProvider( ILogger<CustomConfigProvider> logger,
                                     string pathToJsonConfigFile )
        {
            _logger = logger;

            _jsonConfigFile = new FileInfo(pathToJsonConfigFile);
            if ( ! _jsonConfigFile.Exists )
            {
                string msg=$"FATAL: Could not find Configuration-File '{pathToJsonConfigFile}'";
                logger?.LogCritical(msg);
                throw new WDFatalException(msg);
            }

            try
            {
                string myConfigFileContents = File.ReadAllText(pathToJsonConfigFile);
                _jsonConfig = JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,string>>>(myConfigFileContents);
            }
            catch (JsonException)
            {
                string msg=$"FATAL: Configuration-File '{_jsonConfigFile.FullName}' could not be deserialized to 'Dictionary<string,Dictionary<string,string>>'.";
                logger?.LogCritical(msg);
                throw new WDFatalException(msg);
            }

            // Find out operating system and store corresponding section in osSection
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _osSection=MAGICSTRINGS.LINUX_SECTION;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _osSection=MAGICSTRINGS.MAC_OS_SECTION;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _osSection=MAGICSTRINGS.WINDOWS_SECTION;
            }
            else
            {
                string msg = "Operating System not supported";
                logger?.LogCritical(msg);
                throw new WDFatalException(msg);
            }

            // Check for the presence of the required sections (plaftorm-independent
            // and specific to the used operating-system).
            foreach (string section in new[] { 
                        MAGICSTRINGS.PLATFORM_INDEPENDENT_SECTION, _osSection, } )
            {
                if ( ! _jsonConfig.ContainsKey(section))
                {
                    string msg = $"Config-File '{_jsonConfigFile.FullName}' doesn't contain the required section '{section}'";
                    logger?.LogCritical(msg);
                    throw new WDFatalException(msg);
                }
            }
        }

        /// <summary>
        /// Gets a value for the given key. The section "PlatformIndependent" and
        /// the section for the current operating-system are searched for the key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The value corresponding to the given key.</returns>
        /// <exception cref="WirelessDisplay.Common.WDException">
        /// Thrown, if the key is not found.
        /// </exception>
        public string GetValue(string key)
        {
            foreach (string section in new[] { 
                        MAGICSTRINGS.PLATFORM_INDEPENDENT_SECTION, _osSection, } )
            {
                if ( _jsonConfig[section].ContainsKey(key) )
                {
                    return _jsonConfig[section][key];
                }
            }

            throw new WDException($"Could not find key '{key}' in config-file '{_jsonConfigFile.FullName}'");
        }

        /// <summary>
        /// For easier access of configuration-values.
        /// </summary>
        public string this[string key]
        {
            get => GetValue(key);
        }
        

        /// <summary>
        /// Gets a value for the given key. The section "PlatformIndependent" and
        /// the section for the current operating-system are searched for the key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <param name="defaultValue">If key is not found, this value is returned.</param>
        /// <returns>
        /// The value corresponding to the given key, or the defaultValue, if key
        /// is not present in the config-file.
        /// </returns>
        public string GetValueOrDefault(string key, string defaultValue = default(string) )
        {
            try
            {
                return GetValue(key);
            }
            catch (WDException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Checks, if the given key is present in the configuration-file.
        /// </summary>
        /// <param name="key"> The key to search for. </param>
        /// <returns> True, if the key was found, false otherwise.true </returns>
        public bool ContainsKey(string key)
        {
            try
            {
                GetValue(key);
            }
            catch (WDException)
            {
                return false;
            }

            return true;
        }


    }
}