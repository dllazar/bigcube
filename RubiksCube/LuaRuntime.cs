namespace RubiksCube
{
    using MoonSharp.Interpreter;
    using MoonSharp.Interpreter.Loaders;

    internal static class LuaRuntime 
    {
        private static Script _lua;

        static LuaRuntime()
        {
            Initialize();   
        }
        public static Script Instance { get { return _lua; } }
        public static void ResetInstance() 
        {
            Initialize();
        }
        private static void Initialize()
        {
            _lua = new Script();
            ((ScriptLoaderBase)_lua.Options.ScriptLoader).ModulePaths = new string[] { $"lua/?", "lua/?.lua" };
        }
    }
}
