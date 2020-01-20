using BankLedger.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BankLedger.Services
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
            if (!UserExists(username))
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
                accounts[username] = new Account(username, passwordHasher.HashPassword(username, password), account.Balance, account.Transactions);
            }
            return account;
        }

        public void CreateAccount(string username, string password)
        {
            if (UserExists(username))
            {
                throw new ArgumentException("Username already exists", "username");
            }

            var hashedPassword = passwordHasher.HashPassword(username, password);
            accounts[username] = new Account(username, hashedPassword);
        }

        public bool UserExists(string username)
        {
            return accounts.ContainsKey(username);
        }

        public void UpdateAccount(Account account)
        {
            if (!UserExists(account.Username))
            {
                throw new ArgumentException($"Account with user {account.Username} does not exist", "account");
            }

            accounts[account.Username] = account;
        }
    }
}
