using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DYS.JPay.Shared.Shared.Extensions
{
	public static class JsonExtensions
	{
        public static string Convert(this object value)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter(), },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
            };

            return JsonConvert.SerializeObject(value, settings);
        }

        public static T Convert<T>(this string value)
        {
            if (string.IsNullOrEmpty(value)) return default;
            try
            {
                T result = JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None,
                });

                return result;
            }
            catch
            {
                return default;
            }

        }
    }
}
