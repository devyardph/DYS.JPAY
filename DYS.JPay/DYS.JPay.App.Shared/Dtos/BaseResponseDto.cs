using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DYS.JPay.Common.Dtos
{
    public class BaseResponseDto
    {
        private readonly string _content;
        private readonly HttpStatusCode _statusCode;

        public BaseResponseDto()
        {
        }
        public BaseResponseDto(string content, HttpStatusCode statusCode)
        {
            _content = content;
            _statusCode = statusCode;
        }

        public T Result<T>()
        {
            if (string.IsNullOrEmpty(_content)) return default;
            try
            {
                T result = JsonConvert.DeserializeObject<T>(_content, new JsonSerializerSettings
                {
                    //ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

                return result;
            }
            catch
            {
                return default;
            }

        }

        public HttpStatusCode StatusCode => _statusCode;
        public string Content => _content;
    }
}
