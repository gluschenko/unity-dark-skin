using System;
using System.IO;
using System.Linq;
using UnityDarkSkin.Lib;

namespace UnityDarkSkin
{
    class Program
    {
        public const string AppVersion = "1.4";
        public static ConsoleApp App;

        static void Main()
        {
            Console.Title = $"Unity Dark Skin: Console v{AppVersion}";

            App = new ConsoleApp();
            App.Run();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
