using System;
using System.IO;
using System.Linq;

namespace UnityDarkSkin
{
    class Program
    {
        static SkinType Skin;
        static Arch SystemType;

        static string FilePath;
        static string FileName = "Unity.exe";
        static int BytePosition;
        static string[] Signatures;
        static uint[] InjectionCodes;

        static void Main(string[] args)
        {
            Init();
            Setup();
            Start();
        }

        static void Init()
        {
            Console.Title = "Unity Dark Skin";
            Console.WriteLine("Choose version:");
            Console.WriteLine("Unity.exe (32 bit): type '1'");
            Console.WriteLine("Unity.exe (64 bit): type '2'");
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
        }

        static void Setup()
        {
            switch (SystemType)
            {
                case Arch.x86:
                    Signatures = new string[] {
                        "750433C05EC38B065EC3CCCCCCCCCCCCCCCC",
                        "740433C05EC38B065EC3CCCCCCCCCCCCCCCC",
                    };

                    InjectionCodes = new uint[] {
                        3224568949U,
                        3224568948U,
                    };
                    break;
                case Arch.x64:
                    Signatures = new string[] {
                        "750833C04883C4205BC38B034883C4205BC3",
                        "740833C04883C4205BC38B034883C4205BC3",
                    };

                    InjectionCodes = new uint[] {
                        3224569973U,
                        3224569972U,
                    };
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
                Console.WriteLine("Press any key to change skin...");
                Console.ReadKey();

                BytePosition = GetBytePosition();

                try
                {
                    if (BytePosition != 0)
                    {
                        Console.WriteLine("--------");

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
                    Console.WriteLine("Run application as an Adminisrator");
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
                    SwitchSkin(1);
                    break;
                case SkinType.Dark:
                    SwitchSkin(0);
                    break;
            }
        }

        static void SwitchSkin(int t)
        {
            switch (t)
            {
                case 0:
                    using (BinaryWriter binaryWriter = new BinaryWriter((Stream)File.OpenWrite(FilePath)))
                    {
                        binaryWriter.BaseStream.Position = (long)BytePosition;
                        binaryWriter.Write(InjectionCodes[0]);
                        binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                    Skin = SkinType.Light;
                    break;
                case 1:
                    using (BinaryWriter binaryWriter = new BinaryWriter((Stream)File.OpenWrite(FilePath)))
                    {
                        binaryWriter.BaseStream.Position = (long)BytePosition;
                        binaryWriter.Write(InjectionCodes[1]);
                        binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                    Skin = SkinType.Dark;
                    break;
            }
        }

        static void GetSkinType(int offset)
        {
            using (BinaryReader binaryReader = new BinaryReader((Stream)File.OpenRead(FilePath)))
            {
                binaryReader.BaseStream.Position = (long)offset;
                switch (binaryReader.ReadByte().ToString("X2"))
                {
                    case "75":
                        Skin = SkinType.Light;
                        break;
                    case "74":
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
                foreach (string Signature in Signatures)
                {
                    byte[] SignatureBytes = StringToByteArray(Signature);
                    binaryReader.BaseStream.Position = 0L;
                    byte[] FileBytes = new byte[binaryReader.BaseStream.Length];

                    binaryReader.Read(FileBytes, 0, FileBytes.Length);
                    int pos = FindSignature(FileBytes, SignatureBytes, 0);
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
        static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select((x) => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }

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
