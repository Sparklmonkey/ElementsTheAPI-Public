using System;
using System.Collections.Generic;

namespace ElementsTheAPI.Entities
{
    public class PvpUserStartData
    {
        public List<CardObject> CardDeck { get; set; }
        public List<CardObject> Hand { get; set; }
        public int MarkElement { get; set; }
        public string Username { get; set; }

        public PvpUserStartData(string username, List<CardObject> shuffledDeck, List<CardObject> hand, int markElement)
        {
            Username = username;
            MarkElement = markElement;
            CardDeck = shuffledDeck;
            Hand = hand;
        }
    }
}
