using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Newport
{
    public static class IsolatedStorageHelper
    {
        public static async Task Save(string key, object t)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    var serializer = new XmlSerializer(t.GetType());
                    serializer.Serialize(ms, t);
                    ms.Position = 0;
                    using (var reader = new StreamReader(ms))
                    {
                        var xml = reader.ReadToEnd();
                        var fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, key);
                        await File.WriteAllTextAsync(fullPath, key);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public static async Task<T> Load<T>(string key) where T : class
        {
            var t = default(T);
            try
            {
                var fullPath = Path.Combine(FileSystem.Current.AppDataDirectory, key);
                {
                    var xml = await File.ReadAllTextAsync(fullPath);
                    if (!string.IsNullOrEmpty(xml))
                    {
                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                        {
                            var serializer = new XmlSerializer(typeof(T));
                            t = (T)serializer.Deserialize(ms);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            return t;
        }
    }
}