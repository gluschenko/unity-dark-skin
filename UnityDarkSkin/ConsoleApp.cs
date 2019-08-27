using System;
using System.IO;
using System.Linq;
using UnityDarkSkin.Lib;
using Version = UnityDarkSkin.Lib.Version;

namespace UnityDarkSkin
{
    public class ConsoleApp
    {
        Patcher Patcher;
        string FilePath;
        string EditorFileName = "Unity.exe";

        public void Run()
        {
            string Directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            FilePath = Path.Combine(Directory, EditorFileName);
            Console.WriteLine("\nSearch path: " + FilePath);
            //
            if (File.Exists(FilePath))
            {
                Console.WriteLine("Loading...");
                Patcher = new Patcher(FilePath);

                try {
                    Patcher.Load();
                    OnLoad();
                }
                catch (Exception ex) {
                    Console.WriteLine($"Failed to load {EditorFileName}. Please, try again with Administrator rights\n\n\n{ex}");
                }
            }
            else
            {
                Console.WriteLine($"{EditorFileName} is not found! Please copy this application to one directory with {EditorFileName}");
            }
        }

        void OnLoad()
        {
            ConsoleKeyInfo key;
            Console.WriteLine("Choose your Unity version:");
            Console.WriteLine("Do you want to detect the version automatically or manually? (y/n)");
            key = Console.ReadKey();

            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("");
                Console.WriteLine("Detecting version...");
                Version version = Patcher.DetectVersion();
                if (version != null)
                {
                    Patcher.CurrentVersion = version;
                    OnVersionDetected();
                }
                else
                {
                    Console.WriteLine("This version is not supported. Try another version of Unity");
                }
            }
            else
            {
                Console.WriteLine("");

                var versions = Versions.Get();
                for (int i = 0; i < versions.Length; ++i)
                {
                    Console.WriteLine($"* {versions[i]} => press {i + 1} key");
                }

                while (true)
                {
                    key = Console.ReadKey();

                    if (int.TryParse(key.KeyChar.ToString(), out int ver))
                    {
                        ver--;
                        if (ver >= 0 && ver < versions.Length)
                        {
                            Patcher.CurrentVersion = versions[ver];
                            OnVersionDetected();
                            break;
                        }
                    }
                }
            }
        }

        void OnVersionDetected()
        {
            Console.WriteLine($"\nVesrion: {Patcher.CurrentVersion ?? null}\n");

            Console.WriteLine("Detecting theme...");

            ThemeType theme = Patcher.DetectTheme(Patcher.CurrentVersion);

            if (theme == ThemeType.None)
            {
                Console.WriteLine("Signature is not found");
            }
            else
            {
                Console.WriteLine($"\nCurrent theme: {theme}\n");
            }

            Patch();
        }

        void Patch()
        {
            Console.WriteLine("--------");
            Console.WriteLine("Attention: make backup of your Unity.exe file!");
            Console.WriteLine("Press Enter to change skin...");
            Console.WriteLine("--------");
            Console.ReadKey();
            Console.WriteLine("Please wait...");

            ThemeType newTheme = Patcher.CurrentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
            ThemeType theme = Patcher.SetTheme(newTheme);

            if (newTheme == theme)
            {
                try
                {
                    Patcher.Save();

                    Console.WriteLine("--------");
                    Console.WriteLine($"Congrats! Theme changed into {theme} :D");
                    Console.WriteLine("--------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save {EditorFileName}! Check access permissions.\n\nDetails:\n\n{ex}");
                }
            }
            else
            {
                Console.WriteLine("Failed to switch theme");
            }
        }
    }
}
