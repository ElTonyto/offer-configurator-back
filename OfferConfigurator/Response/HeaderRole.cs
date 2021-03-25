using System;
using OfferConfigurator.Databases;
using OfferConfigurator.Models;
using OfferConfigurator.Services;

namespace OfferConfigurator.Response
{
    public static class HeaderRole
    {
        public static bool Verify(string role)
        {
            return role.Equals("configuration-offer");
        }
    }
}

