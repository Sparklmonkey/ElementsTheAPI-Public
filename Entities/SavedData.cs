using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Entities
{
    public class SavedData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int MarkElement { get; set; }

        public List<CardObject> CurrentDeck { get; set; }
        public List<CardObject> CardInventory { get; set; }
        public int Electrum { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int PlayerScore { get; set; }

        public int CurrentQuestIndex { get; set; }

        //Oracle Vars
        public string NextFalseGod { get; set; }
        public string PetName { get; set; }
        public bool PlayedOracleToday { get; set; }
        public DateTime DayLastOraclePlay { get; set; }

        //Quest 1 Flag
        public bool HasDefeatedLevel0 { get; set; }

        //Quest 2 Flag
        public bool RemovedCardFromDeck { get; set; }

        //Quest 3 Flags
        public bool HasBoughtCardBazaar { get; set; }
        public bool HasSoldCardBazaar { get; set; }

        //Quest 4 Flag
        public bool HasDefeatedLevel1 { get; set; }

        //Quest 5 Flag
        public bool HasDefeatedLevel2 { get; set; }

        public List<string> RedeemedCodes { get; set; }


        public SavedData GetDefault()
        {
            return new SavedData
            {
                MarkElement = 0,
                CurrentDeck = new List<CardObject>(),
                CardInventory = new List<CardObject>(),
                Electrum = 0,
                GamesWon = 0,
                GamesLost = 0,
                PlayerScore = 0,
                CurrentQuestIndex = 0,
                NextFalseGod = "",
                PetName = "",
                PlayedOracleToday = false,
                DayLastOraclePlay = DateTime.Now,
                HasDefeatedLevel0 = false,
                RemovedCardFromDeck = false,
                HasBoughtCardBazaar = false,
                HasSoldCardBazaar = false,
                HasDefeatedLevel1 = false,
                HasDefeatedLevel2 = false,
                RedeemedCodes = new List<string>()
            };
        }
    }
}
