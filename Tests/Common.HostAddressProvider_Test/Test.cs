using System;
using NUnit.Framework;
using WirelessDisplay.Common;

namespace WirelessDisplay.Tests.Common.HostAddressProvider_Test
{
    [TestFixture]
    public class Test
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void HostNameProviderHostName_PrintsHostnameToConsole()
        {
            Assert.DoesNotThrow( ()=>
            {
                string hostname = HostAddressProvider.HostName;
                Console.WriteLine($"Found hostname: '{hostname}'");
            });
        }

        [Test]
        public void HostNameProviderIPv4Address_PrintsIPAddressToConsole()
        {
            Assert.DoesNotThrow( ()=>
            {
                string ipAddrss = HostAddressProvider.IPv4Address;
                Console.WriteLine($"Found hostname: '{ipAddrss}'");
            });
        }


        public static void Main(string[] args)
        {
            Console.WriteLine("Setting up Test");
            var test = new Test();
            test.Setup();

            Console.WriteLine("Starting Tests:");
            test.HostNameProviderHostName_PrintsHostnameToConsole();
            test.HostNameProviderIPv4Address_PrintsIPAddressToConsole();

        }
    }
}