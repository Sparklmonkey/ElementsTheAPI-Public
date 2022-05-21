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

        public int ID { get; set; }
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


        public SavedData()
        {
            MarkElement = 0;
                CurrentDeck = new List<CardObject>();
                CardInventory = new List<CardObject>();
                Electrum = 0;
                GamesWon = 0;
                GamesLost = 0;
                PlayerScore = 0;
                CurrentQuestIndex = 0;
                NextFalseGod = "";
                PetName = "";
                PlayedOracleToday = false;
                DayLastOraclePlay = DateTime.Now;
                HasDefeatedLevel0 = false;
                RemovedCardFromDeck = false;
                HasBoughtCardBazaar = false;
                HasSoldCardBazaar = false;
                HasDefeatedLevel1 = false;
                HasDefeatedLevel2 = false;
                RedeemedCodes = new List<string>();
        }

        public SavedData(SavedDataSQL savedDataSQL)
        {
            MarkElement = savedDataSQL.MarkElement;
            Electrum = Convert.ToInt32(savedDataSQL.Electrum);
            GamesWon = Convert.ToInt32(savedDataSQL.GamesWon);
            GamesLost = Convert.ToInt32(savedDataSQL.GamesLost);
            PlayerScore = Convert.ToInt32(savedDataSQL.PlayerScore);
            CurrentQuestIndex = savedDataSQL.CurrentQuestIndex;
            NextFalseGod = savedDataSQL.NextFalseGod;
            PetName = savedDataSQL.PetName;
            PlayedOracleToday = savedDataSQL.PlayedOracleToday;
            DayLastOraclePlay = DateTime.Now;
            HasDefeatedLevel0 = savedDataSQL.HasDefeatedLevel0;
            RemovedCardFromDeck = savedDataSQL.RemovedCardFromDeck;
            HasBoughtCardBazaar = savedDataSQL.HasBoughtCardBazaar;
            HasSoldCardBazaar = savedDataSQL.HasSoldCardBazaar;
            HasDefeatedLevel1 = savedDataSQL.HasDefeatedLevel1;
            HasDefeatedLevel2 = savedDataSQL.HasDefeatedLevel2;
            if (savedDataSQL.OracleLastDayPlayed == null)
            {
                DayLastOraclePlay = DateTime.Now;
            }
            else
            {
                DayLastOraclePlay = savedDataSQL.OracleLastDayPlayed;
            }
            if(savedDataSQL.RedeemCodeList != null)
            {
                if (savedDataSQL.RedeemCodeList.Contains(","))
                {
                    RedeemedCodes = savedDataSQL.RedeemCodeList.Split(",").ToList();
                }
                else
                {
                    RedeemedCodes = new List<string>();
                    RedeemedCodes.Add(savedDataSQL.RedeemCodeList);
                }
            }
        }
    }
    public class SavedDataSQL
    {

        public int ID { get; set; }
        public int MarkElement { get; set; }

        public string DeckCards { get; set; }
        public string InventoryCards { get; set; }
        public DateTime OracleLastDayPlayed { get; set; }
        public long Electrum { get; set; }
        public long GamesWon { get; set; }
        public long GamesLost { get; set; }
        public long PlayerScore { get; set; }

        public int CurrentQuestIndex { get; set; }

        //Oracle Vars
        public string NextFalseGod { get; set; }
        public string PetName { get; set; }
        public bool PlayedOracleToday { get; set; }

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

        public string RedeemCodeList { get; set; }

    }
}
