using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityDarkSkin.Lib
{
    public class Version
    {
        public readonly string Name;
        public readonly byte[] LightBytes;
        public readonly byte[] DarkBytes;

        public Version(string name, byte[] lightBytes, byte[] darkBytes)
        {
            Name = name;
            LightBytes = lightBytes;
            DarkBytes = darkBytes;
        }

        public byte[] GetBytes(ThemeType skin)
        {
            return skin == ThemeType.Light ? LightBytes : DarkBytes;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
