using System;
using System.IO;
using UnityDarkSkin.Lib;
using Version = UnityDarkSkin.Lib.Version;

namespace UnityDarkSkin
{
    public class ConsoleApp
    {
        Patcher _patcher;
        string _filePath;
        string EditorFileName = "Unity.exe";

        public void Run()
        {
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _filePath = Path.Combine(directory, EditorFileName);
            Console.WriteLine("\nSearch path: " + _filePath);
            //
            if (File.Exists(_filePath))
            {
                Console.WriteLine("Loading...");
                _patcher = new Patcher(_filePath);

                try
                {
                    _patcher.Load();
                    OnLoad();
                }
                catch (Exception ex)
                {
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
            Console.WriteLine("Choose your Unity version:");
            Console.WriteLine("Do you want to detect the version automatically or manually? (y/n)");
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Y)
            {
                Console.WriteLine("");
                Console.WriteLine("Detecting version...");
                Version version = _patcher.DetectVersion();
                if (version != null)
                {
                    _patcher.CurrentVersion = version;
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
                            _patcher.CurrentVersion = versions[ver];
                            OnVersionDetected();
                            break;
                        }
                    }
                }
            }
        }

        void OnVersionDetected()
        {
            Console.WriteLine($"\nVersion: {_patcher.CurrentVersion ?? null}\n");

            Console.WriteLine("Detecting theme...");

            ThemeType theme = _patcher.DetectTheme(_patcher.CurrentVersion);

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

            ThemeType newTheme = _patcher.CurrentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
            ThemeType theme = _patcher.SetTheme(newTheme);

            if (newTheme == theme)
            {
                try
                {
                    _patcher.Save();

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