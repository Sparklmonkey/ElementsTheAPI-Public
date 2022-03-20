using ElementsTheAPI.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Data
{
    public interface IPvpHubContext
    {
        public IMongoCollection<ConnectedUser> ConnectedUserCollection { get; }
        public IMongoCollection<SavedData> SavedDataCollection { get; }
        public IMongoCollection<UserData> UserDataCollection { get; }
    }
}
