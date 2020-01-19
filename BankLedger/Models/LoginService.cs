using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    class LoginService
    {
        private PasswordHasher<string> passwordHasher;
        private AccountManager accountManager;

        public LoginService(PasswordHasher<string> passwordHasher, AccountManager accountManager)
        {
            this.passwordHasher = passwordHasher;
            this.accountManager = accountManager;
        }

        /// <summary>
        /// Checks if the given username/password combination exists and is valid.
        /// If so, returns the account associated with the given username.
        /// </summary>
        /// <param name="username">Username for the account to log in to</param>
        /// <param name="password">Password belonging to the given username</param>
        /// <returns>The Account associated with the given username, if the account
        /// exists and the password is correct. Otherwise, returns null.</returns>
        public Account LogIn(string username, string password)
        {
            if (!accountManager.AccountExists(username))
            {
                return null;
            }

            var account = accountManager.GetAccount(username);
            var verify = passwordHasher.VerifyHashedPassword(username, account.HashedPassword, password);
            if (PasswordVerificationResult.Failed == verify)
            {
                return null;
            }
            else //if (PasswordVerificationResult.SuccessRehashNeeded == verify) TODO handle rehash needed
            {
                return account;
            }
        }

        public bool LogOut()
        {
            return true;
        }
    }
}
