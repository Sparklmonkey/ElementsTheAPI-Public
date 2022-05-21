using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Entities
{
    public class CardObject
    {
        public string CardName { get; set; }
        public string CardType { get; set; }
        public bool IsUpgraded { get; set; }

        public CardObject(CardObjectSQL card)
        {
            CardName = card.CardName;
            CardType = card.CardType;
            IsUpgraded = card.IsUpgraded;
        }
    }

    public class CardObjectSQL
    {
        public int ID { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public bool IsUpgraded { get; set; }
    }
}
