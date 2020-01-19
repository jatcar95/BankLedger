using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    class BankLedger
    {
        private AccountManager accountManager;
        private Account currentAccount;

        public BankLedger(AccountManager accountManager)
        {
            this.accountManager = accountManager;
            this.currentAccount = null;
        }

        public bool LogIn(string username, string password)
        {
            currentAccount = accountManager.LogIn(username, password);
            return IsLoggedIn();
        }

        public bool IsLoggedIn()
        {
            return null == currentAccount;
        }
    }
}
