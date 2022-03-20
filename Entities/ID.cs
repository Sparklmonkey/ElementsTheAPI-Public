using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Entities
{
    public class ID
    {
        public OwnerEnum Owner { get; set; }
        public FieldEnum Field { get; set; }
        public int Index { get; set; }
    }

    public enum OwnerEnum
    {
        Player,
        Opponent
    }
    public enum FieldEnum
    {
        Hand,
        Creature,
        Passive,
        Permanent,
        Player
    }
}
