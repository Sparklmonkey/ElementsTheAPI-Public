using System;
using System.Collections.Generic;

namespace ElementsTheAPI.Entities
{
    public class PvpUserInfo
    {
        public int Score { get; set; }
        public int Win { get; set; }
        public int Lost { get; set; }
        public string Username { get; set; }
        public int ElementMark { get; set; }

        public PvpUserInfo(string username, int wins, int loses, int score, int markElement)
        {
            Username = username;
            Score = score;
            Win = wins;
            Lost = loses;
            ElementMark = markElement;
        }

        public PvpUserInfo(ConnectedUser connectedUser)
        {
            Username = connectedUser.Username;
            Score = connectedUser.Score;
            Win = connectedUser.Win;
            Lost = connectedUser.Lose;
            ElementMark = connectedUser.ElementMark;
        }
    }
}
