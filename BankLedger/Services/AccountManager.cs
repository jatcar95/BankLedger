using BankLedger.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BankLedger.Services
{
    /// <summary>
    /// Manages user accounts, and provides various functionality
    /// for interacting with the accounts.
    /// </summary>
    public class AccountManager
    {
        private PasswordHasher<string> passwordHasher;
        private Dictionary<string, Account> accounts;

        /// <summary>
        /// Creates a new AccountManager
        /// </summary>
        /// <param name="passwordHasher">The PasswordHasher to be used for hasing user passwords</param>
        public AccountManager(PasswordHasher<string> passwordHasher)
        {
            if (null == passwordHasher)
            {
                throw new ArgumentNullException(nameof(passwordHasher));
            }
            this.passwordHasher = passwordHasher;
            accounts = new Dictionary<string, Account>();
        }

        /// <summary>
        /// Returns the Account associated with the given username and password.
        /// If the user does not exist or the password is incorrect, returns null;
        /// </summary>
        /// <param name="username">The username for the account to log in to</param>
        /// <param name="password">The password for the account</param>
        /// <returns>The Account matching the given login info, or null if the user
        /// doesn't exist or the password is incorrect.</returns>
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
                account = new Account(username, passwordHasher.HashPassword(username, password), account.Balance, account.Transactions);
                accounts[username] = account;
            }
            return account;
        }

        /// <summary>
        /// Adds a new account to the AccountManager with the given username and password.
        /// </summary>
        /// <param name="username">The username for the new account</param>
        /// <param name="password">The password for the new account</param>
        public void CreateAccount(string username, string password)
        {
            if (null == username)
            {
                throw new ArgumentNullException(nameof(username));
            }
            if (null == password)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (UserExists(username))
            {
                throw new ArgumentException("Username already exists", "username");
            }

            var hashedPassword = passwordHasher.HashPassword(username, password);
            accounts[username] = new Account(username, hashedPassword);
        }

        /// <summary>
        /// Returns true if there is an account with the given username,
        /// false otherwise.
        /// </summary>
        /// <param name="username">The username to check for existence</param>
        /// <returns>True if account exists with the given username,
        /// false otherwise.</returns>
        public bool UserExists(string username)
        {
            if (null == username)
            {
                throw new ArgumentNullException(nameof(username));
            }
            return accounts.ContainsKey(username);
        }

        /// <summary>
        /// If an account exists with the same username as the given account, replaces
        /// the existing account with the given one. If no such user exists, throws an 
        /// ArgumentException. If the parameter passed is null, throws an ArgumentNullException.
        /// </summary>
        /// <param name="account">The account to replace an existing account with</param>
        public void UpdateAccount(Account account)
        {
            if (null == account)
            {
                throw new ArgumentNullException(nameof(account));
            }
            if (!UserExists(account.Username))
            {
                throw new ArgumentException($"Account with user {account.Username} does not exist", nameof(account));
            }

            accounts[account.Username] = account;
        }
    }
}
