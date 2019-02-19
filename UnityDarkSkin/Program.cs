using System;
using System.IO;
using System.Linq;

namespace UnityDarkSkin
{
    class Program
    {
        static SkinType Skin;
        static Arch SystemType;
        static bool isNewUnity;

        static string FilePath;
        static string FileName = "Unity.exe";
        static int BytePosition;

        static byte[][] bytes;

        static void Main(string[] args)
        {
            Init();
            Setup();
            Start();
        }

        static void Init()
        {
            Console.Title = "Unity Dark Skin v1.1";
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
                Console.WriteLine("* 5.0 to 2018.2: type '1'");
                Console.WriteLine("* 2018.3 to 2019.1: type '2'");
                Console.Write("Your answer: ");

                key = Console.ReadKey();
                switch (key.KeyChar)
                {
                    case '1':
                        isNewUnity = false;
                        break;
                    case '2':
                    default:
                        isNewUnity = true;
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

                    if (!isNewUnity)
                    {
                        bytes = new byte[][] {
                            new byte[] { 0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3 },
                            new byte[] { 0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x20, 0x5B, 0xC3 }
                        };
                    }
                    else
                    {
                        bytes = new byte[][] {
                            new byte[] { 0x75, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x30 },
                            new byte[] { 0x74, 0x08, 0x33, 0xC0, 0x48, 0x83, 0xC4, 0x30, 0x5B, 0xC3, 0x8B, 0x03, 0x48, 0x83, 0xC4, 0x30 }
                        };
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

                        GetSkinType(BytePosition);
                        Console.WriteLine("Current skin: " + Skin.ToString());

                        Console.WriteLine("Please wait...");
                        ToggleSkinType();

                        GetSkinType(BytePosition);
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

        static void ChooseFolder()
        {
            Console.WriteLine("\n--------");
            Console.WriteLine("Is Unity.exe with this .exe?");
            Console.WriteLine("Unity.exe is here!: type 'y'");
            Console.WriteLine("I'll choose my Unity.exe folder: type 'n'");
            Console.Write("Your answer: ");

            ConsoleKeyInfo key = Console.ReadKey();
            string UnityFolder = "";
            switch (key.KeyChar)
            {
                case 'n':
                    Console.Write("\nEnter your file path: ");
                    UnityFolder = Console.ReadLine();
                    break;
                case 'y':
                default:
                    UnityFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    break;
            }
            
            FilePath = UnityFolder + @"\" + FileName;
        }

        static void ToggleSkinType()
        {
            switch (Skin)
            {
                case SkinType.Light:
                    ChangeSkin(SkinType.Dark);
                    break;
                case SkinType.Dark:
                    ChangeSkin(SkinType.Light);
                    break;
            }
        }

        static void ChangeSkin(SkinType skin)
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

        static void GetSkinType(int offset)
        {
            using (BinaryReader binaryReader = new BinaryReader((Stream)File.OpenRead(FilePath)))
            {
                binaryReader.BaseStream.Position = (long)offset;
                switch (binaryReader.ReadByte())
                {
                    case 0x75:
                        Skin = SkinType.Light;
                        break;
                    case 0x74:
                        Skin = SkinType.Dark;
                        break;
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
        }

        // Hex string to array of bytes
        /*static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select((x) => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }*/

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
    }
}
