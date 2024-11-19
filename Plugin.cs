using BetterSongList;
using IPA;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using IPALogger = IPA.Logging.Logger;

[assembly: AssemblyTitle("BetterSongList PlayCount Sort")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License - Copyright © 2024 Steffan Donal")]

[assembly: Guid("f1378c34-e815-4e04-9471-8be443b18e96")]

namespace PlayCount
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        static PlayCountSorter PlayCountSorter => new PlayCountSorter();

        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        public static IPALogger Log { get; internal set; }


        static bool _isInitialized;

        [Init]
        public void Init(object _, IPALogger log)
        {
            Log = log;

            SortMethods.RegisterPrimitiveSorter(PlayCountSorter);
        }

        [OnStart]
        public void OnStart()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"Plugin had {nameof(OnStart)} called more than once! Critical failure.");

            _isInitialized = true;

            Log.Info($"v{Version} loaded!");
        }
    }
}
