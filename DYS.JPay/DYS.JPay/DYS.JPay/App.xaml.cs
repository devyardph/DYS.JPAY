using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Extensions;
using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.Settings;

namespace DYS.JPay
{
    public partial class App : Application
    {
        private readonly IPeerService _peerService;
        private readonly ITransactionService _transactionService;
        private readonly SessionService _sessionService;
        public App(IPeerService peerService,
               ITransactionService transactionService,
               SessionService sessionService)
        {
            InitializeComponent();
            // Global subscription: always active
            _peerService = peerService;
            _transactionService = transactionService;
            _sessionService = sessionService;

            // ✅ Subscribe to peer events
            _peerService.OrderReceived += OnOrderReceived;
            _peerService.PeersUpdated += OnPeersUpdated;
            _peerService.PeerNamesUpdated += OnPeerNamesUpdated;
        }

        // Handle incoming orders
        private void OnOrderReceived(string content)
        {
            Console.WriteLine($"App received order: {content}");

            // Distinguish between order vs status by checking JSON
            if (content.Contains(GlobalSettings.PREPARING) 
             || content.Contains(GlobalSettings.READY) ||
                content.Contains(GlobalSettings.COMPLETED))
            {
                // Status update
                var transaction = JsonExtensions.Convert<TransactionDto>(content);
                //Console.WriteLine($"Order {statusUpdate.OrderId} updated to {statusUpdate.Status}");

                // UPDATE THE STATUS IN PEERS DEVICES TO SEE THAT ORDER HAS BEEN UPDATED FROM THE MAIN DEVICE
                //_transactionService.UpdateTransactionAsync(transaction);
               
            }
            else
            {
                // SAVE THE ORDER WITHIN MAIN DEVICES
                var cart = JsonExtensions.Convert<CartDto>(content);
                _transactionService.PlaceTransactionAsync(cart);
            }
        }

        // Handle peer count changes
        private void OnPeersUpdated(int count)
        {
            Console.WriteLine($"Peers connected: {count}");
            // Update UI or session service
        }

        // Handle peer name list changes
        private void OnPeerNamesUpdated(List<string> names)
        {
            Console.WriteLine("Connected peers: " + string.Join(", ", names));
            // Update UI with peer list
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "DYS.JPay" };
        }
    }
}
