using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Services;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Platforms.iOS
{

    public class PrinterService : IPrinterService
    {
        public bool IsConnected => throw new NotImplementedException();

        public Task<bool> ConnectAsync(string deviceAddress)
        {
            throw new NotImplementedException();
        }

        public Task DisconnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> PrintAsync(byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<List<BluetoothDeviceDto>> ScanDevicesAsync()
        {
            throw new NotImplementedException();
        }

        Task<OutputDto<ConnectionDto>> IPrinterService.ConnectAsync(string deviceAddress)
        {
            throw new NotImplementedException();
        }

        Task<OutputDto<List<BluetoothDeviceDto>>> IPrinterService.ScanDevicesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
