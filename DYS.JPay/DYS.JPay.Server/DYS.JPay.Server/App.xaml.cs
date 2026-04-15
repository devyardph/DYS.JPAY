using DYS.JPay.Server.Services;

namespace DYS.JPay.Server
{
    public partial class App : Application
    {
        public App(ApplicationServer server)
        {
            InitializeComponent();
            server.Start();
        }


        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "DYS.JPay.Server" };
        }
    }
}
