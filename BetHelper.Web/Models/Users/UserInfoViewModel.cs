﻿namespace BetHelper.Web.Models.Users
{
    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool IsDriver { get; set; }

        public string Car { get; set; }
        public bool IsAdmin { get; set; }
    }
}