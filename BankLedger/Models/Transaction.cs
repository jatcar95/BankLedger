using BankLedger.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    class Transaction
    {
        public Transaction(UserAction action, string description = null)
        {
            Action = action;
            Description = description;
            Time = DateTimeOffset.Now;
        }

        public UserAction Action { get; private set; }

        public string Description { get; private set; }

        public DateTimeOffset Time { get; private set; }
    }
}
