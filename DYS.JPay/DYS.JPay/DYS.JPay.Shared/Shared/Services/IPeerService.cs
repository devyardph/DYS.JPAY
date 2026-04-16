using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IPeerService
    {
        void SendOrder(string orderJson);
        void UpdateOrderStatus(string orderId, string status);

        event Action<string> OrderReceived;
        event Action<int> PeersUpdated;
        event Action<List<string>> PeerNamesUpdated; 

    }

}
