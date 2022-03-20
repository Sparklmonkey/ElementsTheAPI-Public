using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Entities
{
    public class PvP_Action
    {
        public int ActionType { get; set; }
        public ID OriginId { get; set; }
        public ID TargetId { get; set; }
    }
}
