using DYS.JPay.Shared.Shared.Entities;
using DYS.JPay.Shared.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DYS.JPay.Shared.Shared.Providers
{
    public class IdentityAuthenticationStateProvider: AuthenticationStateProvider, IIdentityAuthenticationStateProvider
    {
        private readonly SessionService _session;

        public IdentityAuthenticationStateProvider(SessionService session)
        {
            _session = session;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity;

            if (_session.IsLoggedIn)
            {
                identity = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, _session.CurrentUser.Username),
                new Claim(ClaimTypes.Role, _session.CurrentUser.Role)
            }, "CustomAuth");
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public async Task MarkedAsLoggedIn(User user) => _session.SetUser(user);
        public async Task MarkedAsLoggedOut() => _session.ClearUser();
    }
}
