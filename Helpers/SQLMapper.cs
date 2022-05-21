using ElementsTheAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Helpers
{
    public static class SQLMapper
    {
        public static SavedData MapSQLToSavedDataUnity(SavedDataSQL savedDataSQL, List<CardObject> deckList, List<CardObject> inventoryCards)
        {
            SavedData result = new SavedData(savedDataSQL);
            result.CurrentDeck = deckList;
            result.CardInventory = inventoryCards;
            return result;
        }
    }
}
