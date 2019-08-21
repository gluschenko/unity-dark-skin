using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityDarkSkin.Lib
{
    public struct Version
    {
        public string Name { get; set; }
        public byte[] LightBytes { get; set; }
        public byte[] DarkBytes { get; set; }

        public Version(string name, byte[] lightBytes, byte[] darkBytes)
        {
            Name = name;
            LightBytes = lightBytes;
            DarkBytes = darkBytes;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
