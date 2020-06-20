

namespace WirelessDisplay.Common
{
    public static class MagicStrings
    {

        //#####################################################################
        // Magic strings for custom configuration file
        //#####################################################################
        #region 

        // Filename of config.json (the same filename is used in all projects)
        public const string CONFIG_FILE ="config.json";

        // Sections in config.json (used for all config-json-files in all projects)
        public const string PLATFORM_INDEPENDENT_SECTION="PlatformIndependent";
        public const string LINUX_SECTION = "Linux";
        public const string MAC_OS_SECTION = "macOS";
        public const string WINDOWS_SECTION = "Windows";

        // Keys in config.json for project ScriptingRestApiServer
        public const string SHELL = "Shell";
        public const string SHELL_ARGS_TEMPLATE = "ShellArgsTemplate";
        public const string SCRIPT_DIRECTORY = "ScriptDirectory";
        public const string SCRIPT_FILE_EXTENSION = "ScriptFileExtension";

        // Keys in config.json for project WirelessDisplayServer
        public const string PATH_TO_SCRIPTING_REST_API_SERVER = "PathToScriptingRestApiServer";
        public const string ARGS_TEMPLATE_FOR_SCRIPTING_REST_API_SERVER = "ArgsTemplateForScriptingRestApiServer";
        public const string PORT_NUMBERS = "PortNumbers";

        // Keys in config.json for project WirelessDisplayClient
        public const string STREAMING_TYPES = "StreamingTypes";
        public const string PREFERRED_SERVER_PORT = "PreferredServerPort";
        public const string PREFERRED_STREAMING_PORT = "PreferredStreamingPort";
        public const string INDEX_OF_PREFERRED_STREAMING_TYPE = "IndexOfpreferredStreamingType";
        public const string PREFERRED_LOCAL_SCREEN_WIDTH = "PreferredLocalScreenWidth";
        public const string PREFERRED_REMOTE_SCREEN_WIDTH = "PreferredRemoteScreenWidth";

        //
        // Placeholders for shell, scripts and command-line-arguments:
        //
        public const string SCRIPTNAME_MANAGE_SCREEN_RESOLUTION ="ScriptNameManageScreenResolutions";
        public const string SCRIPTARGS_MANAGE_SCREEN_RESOLUTION = "ScriptArgsManageScreenResolutions";
        public const string SCRIPTNAME_START_STREAMING_SINK = "ScriptNameStartStreamingSink";
        public const string SCRIPTARGS_START_STREAMING_SINK = "ScriptArgsStartStreamingSink";
        public const string SCRIPTNAME_START_STREAMING_SOURCE = "ScriptNameStartStreamingSource";
        public const string SCRIPTARGS_START_STREAMING_SOURCE = "ScriptArgsStartStreamingSource";
        public const string SCRIPTNAME_PREVENT_DISPLAY_BLANKING = "ScriptNamePreventDisplayBlanking";
        public const string SCRIPTARGS_PREVENT_DISPLAY_BLANKING = "ScriptArgsPreventDisplayBlanking";
        public const string PLACEHOLDER_SCRIPT_PATH="{SCRIPT}";
        public const string PLACEHOLDER_SCRIPT_ARGS="{SCRIPT_ARGS}";
        public const string PLACEHOLDER_IP = "{IP}";
        public const string PLACEHOLDER_PORT = "{PORT}";
        public const string PLACEHOLDER_ACTION = "{ACTION}";
        public const string ACTION_GET = "GET";
        public const string ACTION_ALL = "ALL";
        public const string ACTION_SET = "SET";
        public const string PLACEHOLDER_SCREEN_RESOLUTION = "{SCREEN_RESOLUTION}";
        public const string PLACEHOLDER_STREAMING_TYPE ="{STREAMING_TYPE}";
        public const string STREAMING_METHOD_VNC = "VNC";
        public const string STREAMING_METHOD_FFMPEG = "FFmpeg";
        public const string PLACEHOLDER_SECONDS = "{SECONDS}";

        #endregion

        //#####################################################################
        // REST-API-paths in the url used in 
        //ScriptRunnerController.cs and in RemoteScriptRunner.cs
        //#####################################################################
        #region 

        public const string RESTAPI_MAIN_PATH="api/ScriptRunner";
        public const string RESTAPI_RUNSCRIPT="RunScript";
        public const string RESTAPI_STARTSCRIPT="StartScript";
        public const string RESTAPI_IS_SCRIPT_RUNNING="IsScriptRunning";
        public const string RESTAPI_STOPSCRIPT="StopScript";

        #endregion
        
    }


}
