using DYS.JPay.Shared.Services;
using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Services;
using System.IO.Ports;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;


namespace DYS.JPay.Platforms.Windows
{
    //COM9 or COM5
    public class PrinterService : IPrinterService
    {
        private SerialPort _serialPort;

        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        public async Task<OutputDto<List<BluetoothDeviceDto>>> ScanDevicesAsync()
        {
            var output = new OutputDto<List<BluetoothDeviceDto>>();
            // On Windows, paired Bluetooth printers appear as COM ports
            var ports = SerialPort.GetPortNames();
            var printers = new List<BluetoothDeviceDto>();

            foreach (var port in ports)
            {
                printers.Add(new BluetoothDeviceDto
                {
                    Name = $"Printer on {port}",
                    Address = port,
                    IsPaired = true
                });
            }
            
            output.Success = true;
            output.Entity = printers;
            return output;
        }

        public async Task<OutputDto<ConnectionDto>> ConnectAsync(string devicePort)
        {
            var output = new OutputDto<ConnectionDto>();
            var connection = new ConnectionDto();
            connection.Connected = false;

            try
            {
                _serialPort = new SerialPort(devicePort, 9600, Parity.None, 8, StopBits.One);
                _serialPort.Handshake = Handshake.None;

                _serialPort.Open();
                output.Success = true;
                connection.Connected = true;
            }
            catch (Exception ex)
            {
                connection.Connected = false;
                output.Success = false;
                output.Message = $"Failed to connect to printer on {devicePort}: {ex.Message}";
            }

            output.Success = connection.Connected;
            output.Entity = connection;
            return output;
        }

        public async Task<bool> PrintAsync(byte[] data)
        {
            if (!IsConnected) return false;

            try
            {
                _serialPort.Write(data, 0, data.Length);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Print failed: {ex.Message}");
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                    _serialPort.Close();

                _serialPort = null;
            }
            catch { }

            await Task.CompletedTask;
        }
    }

}
