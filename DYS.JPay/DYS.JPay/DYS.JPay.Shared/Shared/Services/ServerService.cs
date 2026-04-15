using DYS.JPay.Shared.Shared.Dtos;
using DYS.JPay.Shared.Features.AppSettings.Views;
using DYS.JPay.Shared.Shared.Providers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{

    public interface IServerService : IBaseService
    {

        #region PATIENTS
        Task<BaseResponseDto> SubmitTestOrders(List<TestingDto> payload);
        #endregion
    }
    public class ServerService : BaseService, IServerService
    {
        private readonly IRequestProvider _requestProvider;
        private readonly NavigationManager _navigationManager;
        public ServerService(IRequestProvider requestProvider,
                            NavigationManager navigationManager
            )
        {
            _navigationManager = navigationManager;
            _requestProvider = requestProvider;
            _requestProvider.BaseURI = $"http://192.168.68.60:5000/jpay";
        }

        #region TESTING
        public async Task<BaseResponseDto> SubmitTestOrders(List<TestingDto> payload)
        {
            var api = $"/process";
            var response = await _requestProvider.PostAsync<BaseResponseDto>(api, payload);
            return response;
        }
        #endregion

    }
}
