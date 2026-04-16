using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Services;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;


namespace DYS.JPay.Platforms.Windows
{

    public class PeerService : IPeerService
    {
        private TcpListener _listener;
        private List<TcpClient> _clients = new();

        public event Action<string> OrderReceived;
        public event Action<int> PeersUpdated;
        public event Action<List<string>> PeerNamesUpdated;

        public PeerService()
        {
            _listener = new TcpListener(IPAddress.Any, 5000);
            _listener.Start();
            _listener.BeginAcceptTcpClient(OnClientConnected, null);
        }

        private void OnClientConnected(IAsyncResult ar)
        {
            var client = _listener.EndAcceptTcpClient(ar);
            _clients.Add(client);
            PeersUpdated?.Invoke(_clients.Count);
            PeerNamesUpdated?.Invoke(_clients.Select(c => c.Client.RemoteEndPoint.ToString()).ToList());

            var stream = client.GetStream();
            var reader = new StreamReader(stream);
            Task.Run(async () =>
            {
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line != null)
                        OrderReceived?.Invoke(line);
                }
            });

            _listener.BeginAcceptTcpClient(OnClientConnected, null);
        }

        public void SendOrder(string orderJson)
        {
            foreach (var client in _clients)
            {
                var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                writer.WriteLine(orderJson);
            }
        }

        public void UpdateOrderStatus(string orderId, string status)
        {
            var statusJson = $"{{\"orderId\":\"{orderId}\",\"status\":\"{status}\"}}";

            foreach (var client in _clients)
            {
                var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                writer.WriteLine(statusJson);
            }
        }
    }

}
