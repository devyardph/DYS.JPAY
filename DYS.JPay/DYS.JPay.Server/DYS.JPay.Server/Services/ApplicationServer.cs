using DYS.JPay.Common.Dtos;
using DYS.JPay.Common.Extensions;
using DYS.JPay.Server.Helpers;
using DYS.JPay.Server.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


namespace DYS.JPay.Server.Services
{
    public class ApplicationServer
    {
        //private readonly HttpListener _listener;
        private readonly ITestingService _testingService;
        private readonly IHost _host;

        public ApplicationServer(
            ITestingService testingService)
        {
            _testingService = testingService;

           
        }



    }
}
