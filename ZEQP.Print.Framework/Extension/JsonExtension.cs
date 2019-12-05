using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZEQP.Print.Framework
{
    public static class JsonExtension
    {
        public static T ToObject<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
        public static string ToJson(this object model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
