using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public interface IPrintService
    {
        void PrintReceipt(AppSetting setting, CartDto cart);
        Task PrintReceiptAsync(AppSetting setting, CartDto cart);
        Task<bool> TestConnectionAsync();
    }
}
