using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using OfferConfigurator.Models;
using OfferConfigurator.Databases;

namespace OfferConfigurator.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _user;

        public UserService(IOfferConfiguratorDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _user = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<UserResult> Get() {
            List<User> userList = _user.Find(user =>  true).ToList();
            List<UserResult> result = new List<UserResult>();
            foreach(User user in userList)
            {
                result.Add(UserFormat(user));
            }
            return result;
        }

        public UserResult Get(string id) {
            User user = _user.Find<User>(user => user.Id == id).FirstOrDefault();
            if (user == null) return null;
            return UserFormat(user);
        }

        public User GetById(string id) =>
            _user.Find<User>(user => user.Id == id).FirstOrDefault();

        public User GetByEmail(string email) =>
            _user.Find<User>(user => user.Email == email).FirstOrDefault();

        public UserResult Create(UserBody userBody)
        {
            User user = new User
            {
                FirstName = userBody.FirstName,
                LastName = userBody.FirstName,
                Email = userBody.Email,
                Password = userBody.Password,
            };

            _user.InsertOne(user);

            return UserFormat(user);
        }

        public void Update(string id, User userIn) =>
            _user.ReplaceOne(user => user.Id == id, userIn);

        public void Remove(User userIn) =>
            _user.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) =>
            _user.DeleteOne(user => user.Id == id);

        public UserResult UserFormat(User user)
        {
            return new UserResult
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}