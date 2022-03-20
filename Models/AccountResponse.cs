﻿using ElementsTheAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElementsTheAPI.Models
{
    public class AccountResponse
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public SavedData PlayerData { get; set; }
        public ErrorCases ErrorMessage { get; set; }
        public string Token { get; set; }
    }
}
