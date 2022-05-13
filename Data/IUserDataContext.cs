using MongoDB.Driver;
using ElementsTheAPI.Entities;

namespace ElementsTheAPI.Data
{
    public interface IUserDataContext
    {
        IMongoCollection<UserData> UserDataCollection { get; }
        IMongoCollection<SavedData> SavedDataCollection { get; }
        IMongoCollection<LogData> LogCollection { get; }
        IMongoCollection<EnvFlags> EnvFlagCollection { get; }
    }
}
