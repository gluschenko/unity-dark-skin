using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnityDarkSkin.App.Core
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

