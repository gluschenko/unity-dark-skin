using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UnityDarkSkin.Lib
{
    public class Patcher
    {
        public string FilePath { get; private set; }
        public Version CurrentVersion { get; set; }
        public ThemeType CurrentTheme { get; set; }
        public bool IsLoaded { get => Data != null; }
        public byte[] Data { get; private set; }

        private int offset = 0;

        public Patcher(string path)
        {
            FilePath = path;
        }

        public void Reset()
        {
            offset = 0;
        }

        public void Load()
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException("File doesn't exist");

            Data = File.ReadAllBytes(FilePath);
        }

        public ThemeType SetTheme(ThemeType theme)
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException("File doesn't exist");

            if (CurrentVersion == null)
                throw new InvalidOperationException("Version is not detected");

            if (offset == 0)
                DetectTheme(CurrentVersion);

            var bytes = CurrentVersion.GetBytes(theme);
            if (offset + bytes.Length < Data.Length)
            {
                for (int i = 0; i < bytes.Length; ++i)
                {
                    Data[offset + i] = bytes[i];
                }
            }

            return DetectTheme(CurrentVersion);
        }

        // Search in bytes: O(N)
        // Returns offset of first byte in query or -1
        public int Search(byte[] search, int offset = 0)
        {
            if (!IsLoaded)
                throw new InvalidOperationException("File is not loaded");
            //
            int num = -1;
            if (Data.Length > 0 && 
                search.Length > 0 && 
                offset <= Data.Length - search.Length && 
                Data.Length >= search.Length)
            {
                for (int i = offset; i <= Data.Length - search.Length; ++i)
                {
                    if (Data[i] == search[0])
                    {
                        if (Data.Length > 1)
                        {
                            bool flag = true;
                            for (int j = 1; j < search.Length; ++j)
                            {
                                if (Data[i + j] != search[j])
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                num = i;
                                break;
                            }
                        }
                        else
                        {
                            num = i;
                            break;
                        }
                    }
                }
            }
            return num;
        }

        public Version DetectVersion()
        {
            if (!IsLoaded)
                throw new InvalidOperationException("File is not loaded");
            //
            foreach (Version version in Versions.Get())
            {
                int light = Search(version.LightBytes);
                int dark = Search(version.DarkBytes);

                bool isLight = light != -1;
                bool isDark = dark != -1;

                if (isLight || isDark)
                {
                    offset = Math.Max(light, dark);

                    CurrentVersion = version;
                    return version;
                }
            }

            return null;
        }

        public ThemeType DetectTheme(Version version)
        {
            bool light = Search(version.LightBytes, offset) != -1;
            bool dark = Search(version.DarkBytes, offset) != -1;

            if (light || dark)
                CurrentTheme = light ? ThemeType.Light : ThemeType.Dark;
            else
                CurrentTheme = ThemeType.None;

            return CurrentTheme;
        }

        public void Save()
        {
            if (!IsLoaded)
                throw new InvalidOperationException("File is not loaded");

            File.WriteAllBytes(FilePath, Data);
        }

        public void MakeBackup()
        {
            if (!IsLoaded)
                throw new InvalidOperationException("File is not loaded");

            string FileDir = Path.GetDirectoryName(FilePath);
            string FileName = Path.GetFileName(FilePath);

            DateTime date = DateTime.Now;
            string NewFileName = $"Backup_{date.Day}-{date.Month}-{date.Year}_{date.Hour}:{date.Minute}:{date.Second}_{FileName}";
            string NewPath = Path.Combine(FileDir, NewFileName);

            File.WriteAllBytes(NewPath, Data);
        }

        public void RestoreBackup(string file)
        {
            if (!IsLoaded)
                throw new InvalidOperationException("File is not loaded");

            throw new NotImplementedException();
        }
    }

    public enum ThemeType { None = 0, Light = 1, Dark = 2 }
}
