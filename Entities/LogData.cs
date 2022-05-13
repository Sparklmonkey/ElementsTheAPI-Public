using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Entities
{
    public class LogData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string PlayerId { get; set; }
        public string SaveId { get; set; }
        public string Platform { get; set; }
        public string AppVersion { get; set; }
        public string Time { get; set; }
    }
}
