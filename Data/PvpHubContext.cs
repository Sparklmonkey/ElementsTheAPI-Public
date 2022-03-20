using ElementsTheAPI.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Data
{
    public class PvpHubContext : IPvpHubContext
    {
        public PvpHubContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            ConnectedUserCollection = database.GetCollection<ConnectedUser>(configuration.GetValue<string>("DatabaseSettings:ConnectedUsersConnectionName"));
            SavedDataCollection = database.GetCollection<SavedData>(configuration.GetValue<string>("DatabaseSettings:SavedDataCollectionName"));
            UserDataCollection = database.GetCollection<UserData>(configuration.GetValue<string>("DatabaseSettings:UserCollectionName"));
        }
        public IMongoCollection<ConnectedUser> ConnectedUserCollection { get; }
        public IMongoCollection<SavedData> SavedDataCollection { get; }
        public IMongoCollection<UserData> UserDataCollection { get; }
    }
}
