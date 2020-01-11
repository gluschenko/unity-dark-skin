namespace UnityDarkSkin.Lib
{
    public class Version
    {
        private readonly string _name;
        public readonly byte[] LightBytes;
        public readonly byte[] DarkBytes;

        public Version(string name, byte[] lightBytes, byte[] darkBytes)
        {
            _name = name;
            LightBytes = lightBytes;
            DarkBytes = darkBytes;
        }

        public byte[] GetBytes(ThemeType skin)
        {
            return skin == ThemeType.Light ? LightBytes : DarkBytes;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}