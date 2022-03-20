using ElementsTheAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Models
{
    public class AccountRequest
    {
        public string PlayerId { get; set; }
        public string NewEmailAddress { get; set; }
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public SavedData SavedData { get; set; }
        public string Token { get; set; }
    }
}
