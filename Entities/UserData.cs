using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Entities
{
    public class UserData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SavedDataId { get; set; }
        public string EmailAddress { get; set; }
        public string Otp { get; set; }
        public string CodeGenerateTime { get; set; }
        public bool IsVerified { get; set; }
        public string Salt { get; set; }
    }

    public class UserDataSQL
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string UserPassKey { get; set; }
        public string SavedDataId { get; set; }
        public string EmailAddress { get; set; }
        public string Otp { get; set; }
        public string CodeGenerateTime { get; set; }
        public int IsVerified { get; set; }
        public string Salt { get; set; }
    }
}
