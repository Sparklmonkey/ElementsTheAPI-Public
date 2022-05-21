using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ElementsTheAPI.Entities;
using System.Data.SqlClient;

namespace ElementsTheAPI.Data
{
    public class UserDataContext: IUserDataContext
    {
        public UserDataContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            UserDataCollection = database.GetCollection<UserData>(configuration.GetValue<string>("DatabaseSettings:UserCollectionName"));
            SavedDataCollection = database.GetCollection<SavedData>(configuration.GetValue<string>("DatabaseSettings:SavedDataCollectionName"));
            LogCollection = database.GetCollection<LogData>(configuration.GetValue<string>("DatabaseSettings:LogCollectionName"));
            EnvFlagCollection = database.GetCollection<EnvFlags>(configuration.GetValue<string>("DatabaseSettings:EnvFlagsConnectionName"));
            DatabaseSQL = new SqlConnection(configuration.GetValue<string>("ConnectionStrings:Default"));
        }
        public IMongoCollection<UserData> UserDataCollection { get; }
        public IMongoCollection<SavedData> SavedDataCollection { get; }
        public IMongoCollection<LogData> LogCollection { get; }
        public IMongoCollection<EnvFlags> EnvFlagCollection { get; }
        public SqlConnection DatabaseSQL { get; }
    }
}
