using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;

namespace DYS.JPay
{
    public partial class App : Application
    {
        public App(IPeerService peerService, ITransactionService transactionService)
        {
            InitializeComponent();
            // Global subscription: always active
            peerService.OrderReceived += content =>
            {
                var test = content;
                //var order = System.Text.Json.JsonSerializer.Deserialize<Order>(json);
                //orderRepo.SaveOrder(order.Id, order.Status);
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "DYS.JPay" };
        }
    }
}
