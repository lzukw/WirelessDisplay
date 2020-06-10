

namespace WirelessDisplay.Common
{
    public static class MAGICSTRINGS
    {
        // Filename of config.json (the same filename is used in all projects)
        public const string CONFIG_FILE ="config.json";

        // Sections in config.json (used for all config-json-files in all projects)
        public const string PLATFORM_INDEPENDENT_SECTION="PlatformIndependent";
        public const string LINUX_SECTION = "Linux";
        public const string MAC_OS_SECTION = "macOS";
        public const string WINDOWS_SECTION = "Windows";

        // Keys in config.json of project ScriptingRestApiServer
        public const string SHELL = "Shell";
        public const string SHELL_ARGS_TEMPLATE = "ShellArgsTemplate";
        public const string SCRIPT_DIRECTORY = "ScriptDirectory";
        public const string SCRIPT_FILE_EXTENSION = "ScriptFileExtension";

        // Keys in config.json for project WirelessDisplayServer
        public const string PATH_TO_SCRIPTING_REST_API_SERVER = "PathToScriptingRestApiServer";
        public const string ARGS_TEMPLATE_FOR_SCRIPTING_REST_API_SERVER = "ArgsTemplateForScriptingRestApiServer";
        public const string PORT_NUMBERS = "PortNumbers";


        // Placeholders in some values in some config.json-files:

        public const string PLACEHOLDER_SCRIPT_PATH="{SCRIPT}";
        public const string PLACEHOLDER_SCRIPT_ARGS="{SCRIPT_ARGS}";
        public const string PORT_PLACEHOLDER = "{PORT}";

        // REST-API-paths in the url.
        // used in ScriptRunnerController.cs and in RemoteScriptRunner.cs
        public const string RESTAPI_MAIN_PATH="api/ScriptRunner";
        public const string RESTAPI_RUNSCRIPT="RunScript";
        public const string RESTAPI_STARTSCRIPT="StartScript";
        public const string RESTAPI_IS_SCRIPT_RUNNING="IsScriptRunning";
        public const string RESTAPI_STOPSCRIPT="StopScript";
        
    }


}
