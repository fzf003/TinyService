using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Utils
{
    public interface ISerializationService
    {
        string ToSerialization(object obj);

        object Deserialization(string objstr);

        T Deserialization<T>(string objstr);

    }

    public class DefaultSerializationService : ISerializationService
    {


        public string ToSerialization(object obj)
        {
           return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public object Deserialization(string objstr)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(objstr);
        }


        public T Deserialization<T>(string objstr)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(objstr);
        }
    }
}
