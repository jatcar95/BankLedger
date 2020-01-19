using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    class Account
    {
        public Account(string username, string hashedPassword)
        {
            Username = username;
            HashedPassword = hashedPassword;
        }

        public string Username { get; private set; }

        public string HashedPassword { get; private set; }

    }
}
