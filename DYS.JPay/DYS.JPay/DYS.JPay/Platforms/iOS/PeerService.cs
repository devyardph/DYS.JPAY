using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Services;
using Foundation;
using MultipeerConnectivity;
using System.Text.Json;
using UIKit;

namespace DYS.JPay.Platforms.iOS
{

    public class PeerService : NSObject, IPeerService, IMCSessionDelegate
    {
        private MCSession _session;
        private MCPeerID _peerId;
        private MCNearbyServiceAdvertiser _advertiser;
        private MCNearbyServiceBrowser _browser;

        public event Action<string> OrderReceived;
        public event Action<int> PeersUpdated;
        public event Action<List<string>> PeerNamesUpdated;


        public PeerService()
        {
            _peerId = new MCPeerID(UIDevice.CurrentDevice.Name);
            _session = new MCSession(_peerId, null, MCEncryptionPreference.Required);
            _session.Delegate = this;

            _advertiser = new MCNearbyServiceAdvertiser(_peerId, null, "orderservice");
            _advertiser.Delegate = new AdvertiserDelegate(_session, UpdatePeers);
            _advertiser.StartAdvertisingPeer();

            _browser = new MCNearbyServiceBrowser(_peerId, "orderservice");
            _browser.Delegate = new BrowserDelegate(_session, UpdatePeers);
            _browser.StartBrowsingForPeers();

        }

        private void UpdatePeers()
        {
            PeersUpdated?.Invoke(_session.ConnectedPeers.Length);
            var names = _session.ConnectedPeers.Select(p => p.DisplayName).ToList();
            PeerNamesUpdated?.Invoke(names);
        }

        public void SendOrder(string orderJson)
        {
            if (_session.ConnectedPeers.Length == 0) return;

            var data = NSData.FromString(orderJson, NSStringEncoding.UTF8);
            NSError error;
            _session.SendData(data, _session.ConnectedPeers, MCSessionSendDataMode.Reliable, out error);

            if (error != null)
                Console.WriteLine($"Send error: {error.LocalizedDescription}");
            else
                Console.WriteLine("Order sent successfully.");

        }

        // Data received
        [Export("session:didReceiveData:fromPeer:")]
        public void DidReceiveData(MCSession session, NSData data, MCPeerID peerID)
        {
            var message = NSString.FromData(data, NSStringEncoding.UTF8);
            Console.WriteLine($"Order received from {peerID.DisplayName}: {message}");

            // Notify Blazor UI
            OrderReceived?.Invoke(message);


            // ✅ Send ACK back to sender
            var order = JsonSerializer.Deserialize<TransactionDto>(message);
            //order?.Status = "received";

            var ackJson = JsonSerializer.Serialize(order);
            var ackData = NSData.FromString(ackJson, NSStringEncoding.UTF8);
            NSError error;
            session.SendData(ackData, new MCPeerID[] { peerID }, MCSessionSendDataMode.Reliable, out error);

            if (error != null)
                Console.WriteLine($"ACK send error: {error.LocalizedDescription}");
            else
                Console.WriteLine("ACK sent successfully.");

        }

        // State changes (for debugging)
        public void DidChangeState(MCSession session, MCPeerID peerID, MCSessionState state)
        {
            Console.WriteLine($"Peer {peerID.DisplayName} state: {state}");
            PeersUpdated?.Invoke(session.ConnectedPeers.Length);
        }


        public void UpdateOrderStatus(string orderId, string status)
        {
            if (_session.ConnectedPeers.Length == 0) return;

            var statusJson = $"{{\"orderId\":\"{orderId}\",\"status\":\"{status}\"}}";
            var data = NSData.FromString(statusJson, NSStringEncoding.UTF8);
            NSError error;
            _session.SendData(data, _session.ConnectedPeers, MCSessionSendDataMode.Reliable, out error);

            if (error != null)
                Console.WriteLine($"Status send error: {error.LocalizedDescription}");
            else
                Console.WriteLine($"Order {orderId} updated to {status}");
        }



        // Required stubs
        public void DidReceiveStream(MCSession session, NSInputStream stream, MCPeerID peerID) { }
        public void DidStartReceivingResource(MCSession session, string resourceName, MCPeerID peerID, NSProgress progress) { }
        public void DidFinishReceivingResource(MCSession session, string resourceName, MCPeerID peerID, NSUrl localUrl, NSError error) { }
    }

    // Auto‑accept invitations
    public class AdvertiserDelegate : MCNearbyServiceAdvertiserDelegate
    {
        private readonly MCSession _session;
        private readonly Action _updatePeers;

        public AdvertiserDelegate(MCSession session, Action updatePeers)
        {
            _session = session;
            _updatePeers = updatePeers;
        }

        public override void DidReceiveInvitationFromPeer(
            MCNearbyServiceAdvertiser advertiser,
            MCPeerID peerID,
            NSData context,
            MCNearbyServiceAdvertiserInvitationHandler invitationHandler)
        {
            Console.WriteLine($"Auto‑accepting invitation from {peerID.DisplayName}");
            invitationHandler(true, _session); // ✅ auto‑accept
            _updatePeers?.Invoke();
        }
    }

    // Auto‑invite peers when found
    public class BrowserDelegate : MCNearbyServiceBrowserDelegate
    {
        private readonly MCSession _session;
        private readonly Action _updatePeers;

        public BrowserDelegate(MCSession session, Action updatePeers)
        {
            _session = session;
            _updatePeers = updatePeers;
        }

        public override void FoundPeer(MCNearbyServiceBrowser browser, MCPeerID peerID, NSDictionary info)
        {
            Console.WriteLine($"Found peer {peerID.DisplayName}, auto‑inviting...");
            browser.InvitePeer(peerID, _session, null, 30); // ✅ auto‑invite
            _updatePeers?.Invoke();
        }

        public override void LostPeer(MCNearbyServiceBrowser browser, MCPeerID peerID)
        {
            Console.WriteLine($"Lost peer {peerID.DisplayName}");
            _updatePeers?.Invoke();
        }
    }

}
