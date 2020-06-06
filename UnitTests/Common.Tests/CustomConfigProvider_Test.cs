using NUnit.Framework;
using WirelessDisplay.Common;


// dotnet add package Microsoft.Extensions.Logging
// dotnet add package Microsoft.Extensions.Logging.Console
using Microsoft.Extensions.Logging;


namespace WirelessDisplay.UnitTests.Common.Tests
{
    [TestFixture]
    public class CustomConfigProvider_Test
    {
        // The magic strings are keys and values in the used test-Configuration-File
        const string KEY1 = "key1";
        const string KEY3 = "key3";
        const string VALUE1 = "value1";
        const string VALUE3 = "value3";

        const string NOT_PRESENT_KEY = "foo";

        // NOTE: If using dotnet run (and not dotnet test), remove the first 
        //three ../../../ from path.
        const string CONFIG_FILE_PATH = @"../../../testConfig.json";

        CustomConfigProvider _customConfigProvider;


        [SetUp]
        public void Setup()
        {
            // Logging:
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Default", LogLevel.Debug).AddConsole();
            });
            var logger = loggerFactory.CreateLogger<CustomConfigProvider>();

            _customConfigProvider = new CustomConfigProvider(logger, CONFIG_FILE_PATH);
        }

        [Test]
        public void ContainsKey_KeyPresent_ReturnsTrue()
        {
            bool keyIsPresent;

            keyIsPresent = _customConfigProvider.ContainsKey(KEY1);
            Assert.True ( keyIsPresent );

            keyIsPresent = _customConfigProvider.ContainsKey(KEY3);
            Assert.True ( keyIsPresent );
        }

        [Test]
        public void ContainsKey_KeyNotPresent_ReturnsFalse()
        {
            bool keyIsPresent;

            keyIsPresent = _customConfigProvider.ContainsKey(NOT_PRESENT_KEY);
            Assert.False( keyIsPresent );
        }


        [Test]
        public void GetValue_KeyPresent_ReturnsCorrectValue()
        {
            string val;

            val = _customConfigProvider.GetValue(KEY1);
            Assert.AreEqual( val, VALUE1);

            val = _customConfigProvider.GetValue(KEY3);
            Assert.AreEqual( val, VALUE3);
        }

        [Test]
        public void GetValue_KeyNotPresent_ThrowsExpeption()
        {
            Assert.Throws<WDException>( ()=>
            {
                string val;
                val = _customConfigProvider.GetValue(NOT_PRESENT_KEY);
            } );
        }


        [Test]
        public void GetValueOrDefault_KeyPresent_ReturnsCorrectValue()
        {
            string val;

            val = _customConfigProvider.GetValueOrDefault(KEY1);
            Assert.AreEqual( val, VALUE1);

            val = _customConfigProvider.GetValueOrDefault(KEY3);
            Assert.AreEqual( val, VALUE3);
        }
       
        [Test]
        public void GetValueOrDefault_KeyNotPresent_ReturnsDefault()
        {
            string val;
            val = _customConfigProvider.GetValueOrDefault(NOT_PRESENT_KEY);
            Assert.AreEqual(val, default(string) );

            string customDefaultValue ="blah";
            val = _customConfigProvider.GetValueOrDefault(NOT_PRESENT_KEY, customDefaultValue);
            Assert.AreEqual(val, customDefaultValue );
        }


        // The own Main-Method was added, to be able to debug in Visual-Studio-Code
        // (Normally the tests habe to be run with dotnet test, which is not done, but
        // pressing F5 in Visual-Studio-Code does a dotnet run).
        //
        // Now there are several Main()-Methods, which causes a compiler-error. To fix it
        // the following line has to be added to Common.Tests.csproj in the 
        // section <PropertyGroup>:
        // <StartupObject>WirelessDisplay.UnitTests.Common.Tests.CustomConfigProvider_Test</StartupObject>
        public static void Main(string[] args)
        {
            var test = new CustomConfigProvider_Test();
            test.Setup();

            test.ContainsKey_KeyPresent_ReturnsTrue();
            test.ContainsKey_KeyNotPresent_ReturnsFalse();
            test.GetValue_KeyPresent_ReturnsCorrectValue();
            test.GetValue_KeyNotPresent_ThrowsExpeption();
            test.GetValueOrDefault_KeyPresent_ReturnsCorrectValue();
            test.GetValueOrDefault_KeyNotPresent_ReturnsDefault();
        }

    }
}
