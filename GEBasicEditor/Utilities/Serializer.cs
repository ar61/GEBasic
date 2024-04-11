using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GEBasicEditor.Utilities
{
    public static class Serializer
    {
        public static void ToFile<T> (T instance, string path)
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(T));
                using FileStream fs = File.Create(path);
                serializer.WriteObject(fs, instance);
            }
            catch(Exception ex)
            { 
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Unable to Serialize {instance} to {path}");
                throw;
            }
        }

        public static T? FromFile<T>(string path)
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(T));
                using FileStream fs = new FileStream(path, FileMode.Open);
                return (T?)serializer.ReadObject(fs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Unable to Deserialize {path}");
                throw;
            }
        }
    }
}
