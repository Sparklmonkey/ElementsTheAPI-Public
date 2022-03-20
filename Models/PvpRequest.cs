using ElementsTheAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Models
{
    public class PvpRequest
    {
        public string PlayerId { get; set; }
        public string OpponentId { get; set; }
        public List<PvP_Action> ActionList { get; set; }
        public string Token { get; set; }
    }
}
