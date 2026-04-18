using Android.Content;
using Android.Net.Wifi.P2p;
using DYS.JPay.Shared.Shared.Services;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Text;
using static Android.Net.Wifi.P2p.WifiP2pManager;

namespace DYS.JPay.Platforms.Android
{
    public class PeerService : Java.Lang.Object, IPeerService, WifiP2pManager.IChannelListener
    {
        private WifiP2pManager _manager;
        private WifiP2pManager.Channel _channel;

        public event Action<string> OrderReceived;
        public event Action<int> PeersUpdated;
        public event Action<List<string>> PeerNamesUpdated;

        public PeerService(Context context)
        {
            _manager = (WifiP2pManager)context.GetSystemService(Context.WifiP2pService);
            _channel = _manager.Initialize(context, context.MainLooper, this);

  //          _manager.DiscoverPeers(_channel, new ActionListener(
  //    onSuccess: () => Console.WriteLine("Peer discovery started."),
  //    onFailure: reason => Console.WriteLine($"Peer discovery failed: {reason}")
  //));
        }

        public void OnChannelDisconnected()
        {
            // handle channel lost
        }

        public void SendOrder(string orderJson)
        {
            // Typically use a socket connection once peers are connected
            // Example: send via OutputStream of a WifiP2pSocket
        }

        public void UpdateOrder(string content)
        {
            //var statusJson = $"{{\"orderId\":\"{orderId}\",\"status\":\"{status}\"}}";

            // Example: send via OutputStream of a connected socket
            //if (_socket != null && _socket.OutputStream != null)
            //{
            //    var writer = new OutputStreamWriter(_socket.OutputStream);
            //    writer.WriteLine(statusJson);
            //    writer.Flush();
            //}
        }

    }
}
