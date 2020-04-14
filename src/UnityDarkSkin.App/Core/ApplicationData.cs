using System.Runtime.Serialization;

namespace UnityDarkSkin.Core
{
    [DataContract]
    public class ApplicationData
    {
        [DataMember]
        public double WindowWidth;

        [DataMember]
        public double WindowHeight;
    }
}