using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    class AccountManager
    {
        private PasswordHasher<string> passwordHasher;
        private Dictionary<string, Account> accounts;

        public AccountManager(PasswordHasher<string> passwordHasher)
        {
            this.passwordHasher = passwordHasher;
            accounts = new Dictionary<string, Account>();
        }

        public Account LogIn(string username, string password)
        {
            if (!accounts.ContainsKey(username))
            {
                return null;
            }

            var account = accounts[username];
            var verify = passwordHasher.VerifyHashedPassword(username, account.HashedPassword, password);
            if (PasswordVerificationResult.Failed == verify)
            {
                return null;
            }
            else if (PasswordVerificationResult.SuccessRehashNeeded == verify)
            {
                account = new Account(username, passwordHasher.HashPassword(username, password));
                accounts[username] = account;
            }
            return account;
        }
    }
}
