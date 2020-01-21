using BankLedger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Services
{
    /// <summary>
    /// Represents a bank ledger. Allows a user to create an account
    /// and perform various operations on the account. Keeps only one
    /// user logged in at a time.
    /// </summary>
    public class BankLedger
    {
        private AccountManager accountManager;
        private Account currentAccount;

        /// <summary>
        /// Creates a new BankLedger
        /// </summary>
        /// <param name="accountManager">The AccountManager to be used for managing
        /// user accounts</param>
        public BankLedger(AccountManager accountManager)
        {
            this.accountManager = accountManager;
            this.currentAccount = null;
        }

        /// <summary>
        /// Logs in the user with the given username and password. Returns true if
        /// user successfully logged in, or false if the account does not exist or
        /// the password is incorrect
        /// </summary>
        /// <param name="username">Username of the user logging in</param>
        /// <param name="password">Password of the user logging in</param>
        /// <returns>True if the user was sucessfully logged in, false if an account
        /// does not exist with the given username or if the password was incorrect
        /// </returns>
        public bool LogIn(string username, string password)
        {
            currentAccount = accountManager.LogIn(username, password);
            return IsLoggedIn();
        }

        /// <summary>
        /// Creates a new account with the given username and password,
        /// initialized with a balance of 0
        /// </summary>
        /// <param name="username">Username for the new user</param>
        /// <param name="password">Password for the new user</param>
        public void CreateAccount(string username, string password)
        {
            accountManager.CreateAccount(username, password);
        }

        /// <summary>
        /// Records a deposit of the given amount to the current user's account, adding it
        /// to their balance. There must be a user logged in (called BankLedger.LogIn) 
        /// before a deposit can be made. The amount must be >= 0. 
        /// </summary>
        /// <param name="amount">The amount to deposit into the current user's account</param>
        public void Deposit(double amount)
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }
            if (amount < 0)
            {
                throw new ArgumentException("Amount must be positive", "amount");
            }

            currentAccount = currentAccount.Deposit(amount);
            accountManager.UpdateAccount(currentAccount);
        }

        /// <summary>
        /// Records a withdrawal of the given amount from the current user's account,
        /// subtracting it from their balance. There must be a user logged in (called
        /// BankLedger.LogIn) before a withdrawal can be made. The amount must be >= 0,
        /// and must be less than the current balance of the user's account.
        /// </summary>
        /// <param name="amount">The amount to withdraw from the current user's account</param>
        public void Withdrawal(double amount)
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }
            if (amount < 0)
            {
                throw new ArgumentException("Amount must be positive", "amount");
            }
            if (amount > GetCurrentBalance())
            {
                throw new ArgumentException($"Insufficient funds to withdraw given amount. Current balance={GetCurrentBalance()}, amount={amount}", "amount");
            }

            currentAccount = currentAccount.Withdrawal(amount);
            accountManager.UpdateAccount(currentAccount);
        }

        /// <summary>
        /// Returns the balance for the current user. There must be a user logged in
        /// (called BankLedger.LogIn) before the current balance can be retrieved.
        /// </summary>
        /// <returns>The current user's account balance</returns>
        public double GetCurrentBalance()
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }

            return currentAccount.Balance;
        }

        /// <summary>
        /// Returns a List of all the Transactions made on this account. There must be
        /// a user logged in (called BankLedger.LogIn) before the transaction history
        /// can be retrieved.
        /// </summary>
        /// <returns>List of Transactions, representing the transactions made on this account</returns>
        public List<Transaction> GetTransactionHistory()
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }

            return currentAccount.Transactions;
        }

        /// <summary>
        /// Determines if there is a user currently logged in
        /// </summary>
        /// <returns>True if a user has successfully logged in,
        /// false if a user is not logged in or has logged out</returns>
        public bool IsLoggedIn()
        {
            return null != currentAccount;
        }

        /// <summary>
        /// Returns the username for the user currently logged in, or null if there
        /// is no user currently logged in
        /// </summary>
        /// <returns>The username of the user currently logged in, or null if there
        /// is no user currently logged in</returns>
        public string GetCurrentUser()
        {
            return currentAccount?.Username;
        }

        /// <summary>
        /// Determins if a user with the given username has created an account,
        /// even if they are not currently logged in
        /// </summary>
        /// <param name="username">The username to check for</param>
        /// <returns>True if an account exists with the given username, false otherwise</returns>
        public bool UserExists(string username)
        {
            return accountManager.UserExists(username);
        }

        /// <summary>
        /// Logs out the current user. Methods requiring a user to be logged
        /// in will no longer function after LogOut is called, until another
        /// user logs in
        /// </summary>
        public void LogOut()
        {
            currentAccount = null;
        }
    }
}
