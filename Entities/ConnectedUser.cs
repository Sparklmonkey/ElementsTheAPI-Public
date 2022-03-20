using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace ElementsTheAPI.Entities
{
    public class ConnectedUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string PlayerId { get; set; }
        public string ConnectionId { get; set; }
        public string OpponentConnectionId { get; set; }
        public List<CardObject> DeckList { get; set; }
        public int ElementMark { get; set; }
        public int Score { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public ConnectedType ConnectionType { get; set; }
    }
}
