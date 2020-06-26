using System;
using System.Net;

namespace WirelessDisplay.Common
{
    public static class HostAddressProvider
    {
        /// <summary> The hostname of the local computer. </summary>
        public static string HostName 
        { 
            get 
            {
                string hostName = "unknown";
                try 
                {
                    hostName = Dns.GetHostName();
                }
                catch 
                {
                }
                return hostName;
            }
        }


        /// <summary> The IPv4-address of the local computer. </summary>
        public static string IPv4Address
        {
            get
            {
                string ipv4Address = "unknown";
                IPHostEntry ipE = System.Net.Dns.GetHostEntry(HostName);
                foreach (System.Net.IPAddress ip in ipE.AddressList)
                {
                    if ( ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ) //ipv4
                    {
                        ipv4Address = ip.ToString();
                        // Return the first found IPv4-Address - This is most probalby the right one.
                        break; 
                    }
                }
                return ipv4Address;
            }
        }


    }
}