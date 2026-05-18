using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Java.Net; // Android sockets
using Java.IO;
using DYS.JPay.Shared.Shared.Services;  // Java I/O

namespace DYS.JPay.Platforms.Android
{

    public class PeerService : IPeerService
    {
        private ServerSocket _serverSocket;
        private readonly List<Socket> _clients = new();

        public event Action<string> OrderReceived;
        public event Action<int> PeersUpdated;
        public event Action<List<string>> PeerNamesUpdated;

        public PeerService()
        {
            //Task.Run(() => StartServer());
        }

        private async Task StartServer()
        {
            try
            {
                _serverSocket = new ServerSocket(5000); // listen on port 5000

                while (true)
                {
                    var client = await _serverSocket.AcceptAsync(); // wait for connection
                    _clients.Add(client);

                    PeersUpdated?.Invoke(_clients.Count);
                    PeerNamesUpdated?.Invoke(_clients.ConvertAll(c => c.RemoteSocketAddress.ToString()));

                    Task.Run(() => HandleClient(client));
                }
            }
            catch (Exception ex)
            {
                //Android.Util.Log.Error("PeerService", $"Server error: {ex.Message}");
            }
        }

        private async Task HandleClient(Socket client)
        {
            try
            {
                //using var reader = new BufferedReader(new InputStreamReader(client.InputStream));
                //string line;
                //while ((line = await reader.ReadLineAsync()) != null)
                //{
                //    OrderReceived?.Invoke(line);
                //}
            }
            catch (Exception ex)
            {
                //Android.Util.Log.Error("PeerService", $"Client error: {ex.Message}");
            }
        }

        public void SendOrder(string orderJson)
        {
            foreach (var client in _clients)
            {
                try
                {
                    //using var writer = new PrintWriter(client.OutputStream, true);
                    //writer.Println(orderJson);
                }
                catch (Exception ex)
                {
                    //Android.Util.Log.Error("PeerService", $"Send error: {ex.Message}");
                }
            }
        }

        public void UpdateOrder(string content)
        {
            foreach (var client in _clients)
            {
                try
                {
                    //using var writer = new PrintWriter(client.OutputStream, true);
                    //writer.Println(content);
                }
                catch (Exception ex)
                {
                    //Android.Util.Log.Error("PeerService", $"Update error: {ex.Message}");
                }
            }
        }
    }

}
