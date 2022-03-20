using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Entities
{
    public class PvpRoom
    {
        public ConnectedUser FirstConnectedPlayer { get; set; }
        public ConnectedUser SecondConnectedPlayer { get; set; }
        public bool PlayerOneConnected { get; set; }
        public bool PlayerTwoConnected { get; set; }
    }
}
