
using DYS.JPay.Server.Helpers;
using DYS.JPay.Common.Dtos;
using DYS.JPay.Server.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DYS.JPay.Common.Extensions;

namespace DYS.JPay.Server.Services
{
    public class ApplicationServer
    {
        private readonly HttpListener _listener;
        private readonly ITestingService _testingService;

        public ApplicationServer(
            HttpListener listener,
            ITestingService testingService)
        {
            _listener = listener;
            _testingService = testingService;
        }

        public void Start()
        {
            // Configure prefixes once, using the injected listener
            var ip = NetworkHelper.GetLocalWifiIp();
            _listener.Prefixes.Clear();
            _listener.Prefixes.Add("http://+:5000/");

            _listener.Start();

            Task.Run(async () =>
            {
                while (_listener.IsListening)
                {
                    var context = await _listener.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;

                    if (request!.Url!.AbsolutePath == "/jpay/process" && request.HttpMethod == "POST")
                    {
                        using var reader = new StreamReader(request.InputStream);
                        var body = await reader.ReadToEndAsync();
                        var orders = JsonExtensions.Convert<List<TestingDto>>(body);

                        int count = 0;
                        if (orders != null)
                        {
                            foreach (var order in orders)
                            {
                                // Here you could insert into SQLite or process the order
                                count++;
                                var a = new Testing() { Title = order.Title, Description= order.Description };
                                await _testingService.SaveChangesAsync(a); 
                            }
                        }

                        var total = await _testingService.GetAllAsync();
                        string message = $"Received {total?.Count()} orders successfully at {DateTime.Now}.";
                        var buffer = System.Text.Encoding.UTF8.GetBytes(message);

                        response.StatusCode = 200;
                        response.ContentType = "text/plain";
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        response.Close();

                    }
                    else if (request.Url.AbsolutePath == "/jpay/process" && request.HttpMethod == "GET")
                    {
                        string message = $"Received get orders successfully at {DateTime.Now}.";
                        var buffer = System.Text.Encoding.UTF8.GetBytes(message);

                        response.StatusCode = 200;
                        response.ContentType = "text/plain";
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        response.Close();
                    }
                    response.Close();
                }
            });
        }

    }
}
