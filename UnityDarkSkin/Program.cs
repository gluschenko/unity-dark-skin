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

        static void Main(string[] args)
        {
            Console.Title = $"Unity Dark Skin: Console v{AppVersion}";

            App = new ConsoleApp();
            App.Run();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            //Init();
            //Setup();
            //Start();
        }

        /*
         *  LEGACY CODE!!! All this code has been moved to UnityDarkSkin.Lib - universal tool for Console and WPF apps
         * 
        static SkinType Skin;
        static Arch SystemType;
        static UnityVersion Version;

        static string FilePath;
        static string FileName = "Unity.exe";
        static int BytePosition;

        static byte[][] bytes;
        static byte LightByte;
        static byte DarkByte;

        enum Arch
        {
            x86,
            x64,
        }

        enum SkinType
        {
            Dark,
            Light,
        }

        enum UnityVersion
        {
            _2018_2_AND_OLDER,
            _2018_3,
            _2018_4,
            _2019_1,
            _2019_2,
        }

        static void Init()
        {
            Console.Title = $"Unity Dark Skin v{AppVersion}";
            Console.WriteLine("Choose version:");
            Console.WriteLine("* Unity.exe (32 bit): type '1'");
            Console.WriteLine("* Unity.exe (64 bit): type '2'");
            Console.Write("Your answer: ");

            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.KeyChar)
            {
                case '1':
                    SystemType = Arch.x86;
                    break;
                case '2':
                default:
                    SystemType = Arch.x64;
                    break;
            }

            Console.WriteLine("\n");
            Console.WriteLine("----------");

            if (SystemType == Arch.x64)
            {
                Console.WriteLine("Choose your Unity version:");
                Console.WriteLine("* 5.0 - 2018.2: type '1'");
                Console.WriteLine("* 2018.3: type '2'");
                Console.WriteLine("* 2018.4: type '3'");
                Console.WriteLine("* 2019.1: type '4'");
                Console.WriteLine("* 2019.2: type '5'");
                Console.Write("Your answer: ");

                key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case '1':
                        Version = UnityVersion._2018_2_AND_OLDER;
                        break;
                    case '2':
                        Version = UnityVersion._2018_3;
                        break;
                    case '3':
                        Version = UnityVersion._2018_4;
                        break;
                    case '4':
                        Version = UnityVersion._2019_1;
                        break;
                    case '5':
                    default:
                        Version = UnityVersion._2019_2;
                        break;
                }
            }
        }

        static void Setup()
        {
            switch (SystemType)
            {
                case Arch.x86:

                    bytes = new byte[][] {
                        new byte[] { 0x75, 0x04, 0x33, 0xC0, 0x5E, 0xC3, 0x8B, 0x06, 0x5E, 0xC3, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC },
                        new byte[] { 0x74, 0x04, 0x33, 0xC0, 0x5E, 0xC3, 0x8B, 0x06, 0x5E, 0xC3, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC, 0xCC }                       
                    };
                    break;

                case Arch.x64:

                    switch (Version)
                    {
                        case UnityVersion._2018_2_AND_OLDER:
                            LightByte = 0x75;
                            DarkByte = 0x74;
                            bytes = new byte[][] {
                                new byte[] { 0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3 },
                                new byte[] { 0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3 }
                            };
                            break;
                        case UnityVersion._2018_3:
                            LightByte = 0x75;
                            DarkByte = 0x74;
                            bytes = new byte[][] {
                                new byte[] { 0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x30 },
                                new byte[] { 0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x30 }
                            };
                            break;
                        // 000000014127606F  74 04 33 C0 EB 02 8B 03 48 8B 4C 24 58 48 33 CC  t.3Àë...H.L$XH3Ì  
                        case UnityVersion._2018_4:
                            LightByte = 0x74;
                            DarkByte = 0x75;
                            bytes = new byte[][] {
                                new byte[] { 0x74, 0x04, 0x33, 0xC0, 0xEB, 0x02, 0x8B, 0x03, 0x48, 0x8B, 0x4C, 0x24, 0x58, 0x48 },
                                new byte[] { 0x75, 0x04, 0x33, 0xC0, 0xEB, 0x02, 0x8B, 0x03, 0x48, 0x8B, 0x4C, 0x24, 0x58, 0x48 }
                            };
                            break;
                        case UnityVersion._2019_1:
                            LightByte = 0x74;
                            DarkByte = 0x75;
                            bytes = new byte[][] {
                                new byte[] { 0x74, 0x04, 0x33, 0xC0, 0xEB, 0x02, 0x8B, 0x07 },
                                new byte[] { 0x75, 0x04, 0x33, 0xC0, 0xEB, 0x02, 0x8B, 0x07 }
                            };                            
                            break;
                        case UnityVersion._2019_2:
                            LightByte = 0x75;
                            DarkByte = 0x74;
                            bytes = new byte[][] {
                                new byte[] { 0x75, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49 },
                                new byte[] { 0x74, 0x15, 0x33, 0xC0, 0xEB, 0x13, 0x90, 0x49 }
                            };
                            break;
                    }
                    break;
            }
        }

        static void Start()
        {
            string Directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            FilePath = Directory + @"\" + FileName;
            Console.WriteLine("\nSearch path: " + FilePath);

            if (File.Exists(FilePath))
            {
                Console.WriteLine("--------");
                Console.WriteLine("Attention: make backup of your Unity.exe file!");
                Console.WriteLine("Press Enter to change skin...");
                Console.ReadKey();
                Console.WriteLine("Please wait...");

                BytePosition = GetBytePosition();

                try
                {
                    if (BytePosition != 0)
                    {
                        Console.WriteLine("--------");
                        Console.WriteLine("Please wait...");

                        GetSkin(BytePosition);
                        Console.WriteLine("Current skin: " + Skin.ToString());

                        Console.WriteLine("Please wait...");
                        ToggleSkinType();

                        GetSkin(BytePosition);
                        Console.WriteLine("Current skin: " + Skin.ToString());
                    }
                    else
                    {
                        Console.WriteLine("--------");
                        Console.WriteLine("Signature is not found. Choose another file.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("--------");
                    Console.WriteLine("Error is ocurred...");
                    Console.WriteLine("Run application as an Adminisrator: " + e.ToString());
                }

                Console.WriteLine("--------");
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine(FileName + " not found.\nPlease copy this application to folder with " + FileName);
            }

            Console.ReadKey();
        }

        static void ToggleSkinType()
        {
            switch (Skin)
            {
                case SkinType.Light:
                    SetSkin(SkinType.Dark);
                    break;
                case SkinType.Dark:
                    SetSkin(SkinType.Light);
                    break;
            }
        }

        static void SetSkin(SkinType skin)
        {
            switch (skin)
            {
                case SkinType.Light:
                    using (BinaryWriter binaryWriter = new BinaryWriter((Stream)File.OpenWrite(FilePath)))
                    {
                        binaryWriter.BaseStream.Position = (long)BytePosition;
                        binaryWriter.Write(bytes[0]);
                        binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                    Skin = SkinType.Dark;
                    break;
                case SkinType.Dark:
                    using (BinaryWriter binaryWriter = new BinaryWriter((Stream)File.OpenWrite(FilePath)))
                    {
                        binaryWriter.BaseStream.Position = (long)BytePosition;
                        binaryWriter.Write(bytes[1]);
                        binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                    Skin = SkinType.Light;
                    break;
            }
        }

        static void GetSkin(int offset)
        {
            using (BinaryReader binaryReader = new BinaryReader((Stream)File.OpenRead(FilePath)))
            {
                binaryReader.BaseStream.Position = (long)offset;
                byte themeByte = binaryReader.ReadByte();

                if (themeByte == LightByte)
                {
                    Skin = SkinType.Light;
                }
                else
                if (themeByte == DarkByte)
                {
                    Skin = SkinType.Dark;
                }

                binaryReader.Close();
            }
        }

        // Returns offset of signature
        static int GetBytePosition()
        {
            int position = 0;
            using (BinaryReader binaryReader = new BinaryReader((Stream)File.OpenRead(FilePath)))
            {
                foreach (byte[] byteLine in bytes)
                {
                    binaryReader.BaseStream.Position = 0L;
                    byte[] FileBytes = new byte[binaryReader.BaseStream.Length];

                    binaryReader.Read(FileBytes, 0, FileBytes.Length);
                    int pos = FindSignature(FileBytes, byteLine, 0);
                    if (pos != -1)
                    {
                        position = pos;
                        break;
                    }
                }
                binaryReader.Close();
            }
            return position;
        }

        // Searches in an executable file
        static int FindSignature(byte[] bytes, byte[] search, int offset = 0)
        {
            int num = -1;
            if (bytes.Length > 0 && search.Length > 0 && (offset <= bytes.Length - search.Length && bytes.Length >= search.Length))
            {
                for (int i = offset; i <= bytes.Length - search.Length; ++i)
                {
                    if (bytes[i] == search[0])
                    {
                        if (bytes.Length > 1)
                        {
                            bool flag = true;
                            for (int j = 1; j < search.Length; ++j)
                            {
                                if (bytes[i + j] != search[j])
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
        }*/
    }
}
