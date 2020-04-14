using System;
using System.IO;

namespace UnityDarkSkin.Core
{
    public class DataManager<T>
    {
        private string Path { get; set; }

        public DataManager(string path)
        {
            Path = path;
        }

        public T Load(Action<Exception> onError)
        {
            if (File.Exists(Path))
            {
                try
                {
                    string data = File.ReadAllText(Path);
                    return JsonUtility.FromJson<T>(data);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex);
                }
            }

            return Activator.CreateInstance<T>();
        }

        public void Save(T obj, Action<Exception> onError)
        {
            try
            {
                string data = JsonUtility.ToJson(obj);
                File.WriteAllText(Path, data);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }
    }
}