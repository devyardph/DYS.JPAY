using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Providers
{
    public interface IIdentityAuthenticationStateProvider
    {
       public Task MarkedAsLoggedIn(User user);
        public Task MarkedAsLoggedOut();
    }
}
