using System;
using OfferConfigurator.Databases;
using OfferConfigurator.Models;
using OfferConfigurator.Services;

namespace OfferConfigurator.Response
{
    public class VerifyHeaderId
    {
        public UserService userService;

        public VerifyHeaderId(IOfferConfiguratorDatabaseSettings settings)
        {
            userService = new UserService(settings);
        }

        public bool checkUser(string id)
        {
            UserResult user = userService.Get(id);
            if (user == null) return false;
            return true;
        }
    }
}

