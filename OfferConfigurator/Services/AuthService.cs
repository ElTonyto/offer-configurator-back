using System;
using OfferConfigurator.Models;
using OfferConfigurator.Databases;

namespace OfferConfigurator.Services
{
    public class AuthService
    {
        public UserService userService;

        public AuthService(IOfferConfiguratorDatabaseSettings settings)
        {
            userService = new UserService(settings);
        }

        public UserResult Create(UserBody userBody)
        {
            return userService.Create(userBody);
        }

        public string GetHash(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public bool CompareHash(string attemptedPassword, string password) =>
            BCrypt.Net.BCrypt.Verify(attemptedPassword, password);
    }
}