using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    // Interfaces/IBluetoothPrinterService.cs
    public interface IPrinterService
    {
        Task<OutputDto<List<BluetoothDeviceDto>>> ScanDevicesAsync();
        Task<OutputDto<ConnectionDto>> ConnectAsync(string deviceAddress);
        Task DisconnectAsync();
        Task<bool> PrintAsync(byte[] data);
        bool IsConnected { get; }
    }
}
