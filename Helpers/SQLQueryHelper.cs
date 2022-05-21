using ElementsTheAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Helpers
{
    public static class SQLQueryHelper
    {
        public static string GetIDWithCardQuery(CardObject card)
        {
            int isUpgraded = card.IsUpgraded ? 1 : 0;
            return @$"SELECT [ID] 
                    FROM [sparklmo_ElementsDB].[sparklmo_admin].[CardDatabase] 
                    WHERE [cardName] = '{card.CardName}' AND[cardType] = '{card.CardType}' AND[isUpgraded] = {isUpgraded}";
        }

        public static string GetCardWithIDQuery(string cardIDs)
        {
            return $"SELECT * FROM[sparklmo_ElementsDB].[sparklmo_admin].[CardDatabase] WHERE [ID] IN ({cardIDs})";
        }

        public static string GetPlayerDataWithUsername(string username)
        {
            return @$"SELECT [UserPassKey],[SavedDataId],[Salt] 
                    FROM [sparklmo_ElementsDB].[sparklmo_admin].[UserData] WHERE[Username] = '{username}'";
        }

        public static string GetSaveDataWithID(int saveID)
        {
            return $"SELECT * FROM[sparklmo_ElementsDB].[sparklmo_admin].[SavedData]WHERE[ID] = {saveID}";
        }

        public static string UpdateSaveData(SavedDataSQL savedData)
        {
            return $@"UPDATE [sparklmo_ElementsDB].[sparklmo_admin].[SavedData]
SET   markElement = {savedData.MarkElement}
      ,deckCards = {savedData.DeckCards}
      ,inventoryCards = {savedData.InventoryCards}
      ,electrum =  {savedData.Electrum}
      ,gamesWon =  {savedData.GamesWon}
      ,gamesLost =  {savedData.GamesLost}
      ,playerScore =  {savedData.PlayerScore}
      ,currentQuestIndex =  {savedData.CurrentQuestIndex}
      ,nextFalseGod =  {savedData.NextFalseGod}
      ,petName =  {savedData.PetName}
      ,playedOracleToday =  {savedData.PlayedOracleToday}
      ,hasDefeatedLevel0 =  {savedData.HasDefeatedLevel0}
      ,removedCardFromDeck =  {savedData.RemovedCardFromDeck}
      ,hasBoughtCardBazaar =  {savedData.HasBoughtCardBazaar}
      ,hasSoldCardBazaar =  {savedData.HasSoldCardBazaar}
      ,hasDefeatedLevel1 =  {savedData.HasDefeatedLevel1}
      ,hasDefeatedLevel2 = {savedData.HasDefeatedLevel2}
      ,redeemCodeList = {savedData.RedeemCodeList}
      ,OracleLastDayPlayed = {savedData.OracleLastDayPlayed}
WHERE ID = {savedData.ID};";
        }
    }
}
