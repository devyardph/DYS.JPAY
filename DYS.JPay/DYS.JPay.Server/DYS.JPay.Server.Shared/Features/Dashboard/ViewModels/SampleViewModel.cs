using CommunityToolkit.Mvvm.ComponentModel;
using DYS.JPay.Common.Dtos;
using DYS.JPay.Server.Shared.Entities;
using DYS.JPay.Server.Shared.Features.Dashboard.Services;
using DYS.JPay.Server.Shared.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DYS.JPay.Shared.Features.Products.ViewModels
{
    public partial class SampleViewModel : BaseViewModel
    {

        public readonly ISampleService _sampleService;

        public SampleViewModel(NavigationManager navigationManager,
            IJSRuntime jsRuntime,
            ISampleService sampleService) 
            : base(navigationManager, jsRuntime)
        {
            _navigationManager = navigationManager;
            _sampleService = sampleService;
        }

        #region PROPERTIES
        [ObservableProperty]
        private List<Testing> testings = new List<Testing>();
        #endregion

        #region FUNCTIONS
        public async Task GetAllAsync()
        {
            IsBusy = true;
            Testings = await _sampleService.GetAllAsync();
            IsBusy = false;
        }
        #endregion

    }
}
