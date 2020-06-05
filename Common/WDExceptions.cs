using System;

namespace WirelessDisplay.Common
{
    /// <summary>
    /// This type of exception is thrown in this project under normal
    /// circumstances.
    /// </summary>
    public class WDException : Exception
    {
        /// <summary> Constructor. </summary>
        /// <param name="message"> The error message. </param>
        public WDException(string message) : base(message)
        {
        }
    }


    /// <summary>
    /// This type of exception is thrown, when there is a condition that
    /// leads to a program-termination. It's no use catch this type of
    /// exception.
    /// </summary>
    public class WDFatalException : Exception
    {
        public WDFatalException(string message) : base(message)
        {
        }
    }
}