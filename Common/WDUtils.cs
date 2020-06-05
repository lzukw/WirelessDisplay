using System;
using System.Runtime.InteropServices;

namespace WirelessDisplay.Common
{
    /// <summary>
    /// Definition of the supported operating-systems.
    /// </summary>
    public enum Platform
    {
        Linux,
        macOs,
        Windows
    }

    /// <summary>
    /// This class contains helper-methods for general tasks.
    /// </summary>
    public static class WDUtils
    {
        /// <deprecated/>
        /// <summary>
        /// Returns the operating system of the local computer.
        /// </summary>
        /// <returns>
        /// Either Platform.Linux, Platform.macOS or Platform.Windows
        /// </returns>
        public static Platform GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux ))
            {
                return Platform.Linux;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Platform.macOs;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Platform.Windows;
            }
            
            throw new Exception("FATAL: Your Operating System is not supported!");         
        }

    }

}