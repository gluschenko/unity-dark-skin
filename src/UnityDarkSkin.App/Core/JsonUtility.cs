using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace UnityDarkSkin.Core
{
    public class JsonUtility
    {
        public static string ToJson(object obj)
        {
            if (obj == null)
                return "";

            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(obj.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                jsonFormatter.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static object FromJson(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            if (type == null)
                throw new ArgumentNullException("type");

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(type);
                return jsonFormatter.ReadObject(stream);
            }
        }

        public static T FromJson<T>(string json)
        {
            return (T) FromJson(json, typeof(T));
        }

        /*public static void FromJsonOverwrite(string json, object objectToOverwrite)
        {
            if (string.IsNullOrEmpty(json))
                return;

            if (objectToOverwrite == null)
                throw new ArgumentNullException("objectToOverwrite");
        }*/
    }

    /*
    using System.Runtime.Serialization;

    [DataContract]
    public class Person
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Age { get; set; }

        public Person(string name, int year)
        {
            Name = name;
            Age = year;
        }
    }*/
}