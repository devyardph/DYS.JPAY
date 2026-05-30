using Android.Bluetooth;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Services;


namespace DYS.JPay.Platforms.Android
{
    public class PrinterService : IPrinterService
    {
        private BluetoothSocket _socket;
        private BluetoothDevice _device;

        public bool IsConnected => _socket != null && _socket.IsConnected;

        public async Task<OutputDto<List<BluetoothDeviceDto>>> ScanDevicesAsync()
        {
            var output = new OutputDto<List<BluetoothDeviceDto>>();
            var adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null || !adapter.IsEnabled)
            {     
                output.Success = false;
                output.Message = "Bluetooth not available or disabled";
                 return output;
            }

            var printers = new List<BluetoothDeviceDto>();
            foreach (var device in adapter.BondedDevices!)
            {
                printers.Add(new BluetoothDeviceDto
                {
                    Name = device.Name ?? string.Empty,
                    Address = device.Address ?? string.Empty,
                    IsPaired = true
                });
            }
            output.Success = true;
            return output;
        }
        public async Task<OutputDto<ConnectionDto>> ConnectAsync(string model)
        {
            var ouput = new OutputDto<ConnectionDto>();
            var connection = new ConnectionDto();
            connection.Connected = false;
            var devices = await ScanDevicesAsync();
            ouput.Message = devices.Message;
            ouput.Success = devices.Success;
            var device = devices.Entity!.FirstOrDefault(query => query.Name.Contains(model));
            if (device != null)
            {
                var adapter = BluetoothAdapter.DefaultAdapter;
                _device = adapter.GetRemoteDevice(device.Address);

                var sppUuid = Java.Util.UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                _socket = _device.CreateRfcommSocketToServiceRecord(sppUuid);

                await _socket.ConnectAsync();
                Preferences.Set("PrinterMac", device.Address);

                ouput.Message = _socket.IsConnected ? "Connected successfully." : "Failed to connect.";
                ouput.Success = _socket.IsConnected;
                connection.Connected = _socket.IsConnected;
            }
            ouput.Entity = connection;
            return ouput;
        }
        public async Task<bool> PrintAsync(byte[] data)
        {
            if (!IsConnected) return false;

            var outputStream = _socket.OutputStream;
            outputStream?.Write(data, 0, data.Length);
            outputStream?.Flush();

            return true;
        }
        public async Task DisconnectAsync()
        {
            try
            {
                _socket?.Close();
                _socket = null;
            }
            catch { }
        }
    }

}
