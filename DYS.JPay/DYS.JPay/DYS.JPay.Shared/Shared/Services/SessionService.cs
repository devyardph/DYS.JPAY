using DYS.JPay.Shared.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Shared.Shared.Services
{
    public class SessionService
    {
        public User CurrentUser { get; private set; }

        public void SetUser(User user) => CurrentUser = user;
        public void ClearUser() => CurrentUser = null;

        public bool IsLoggedIn => CurrentUser != null;
        public bool IsInRole(string role) => CurrentUser?.Role == role;
    }

}
