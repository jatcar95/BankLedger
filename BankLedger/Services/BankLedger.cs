using BankLedger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Services
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

        public void CreateAccount(string username, string password)
        {
            accountManager.CreateAccount(username, password);
        }

        public void Deposit(double amount)
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }

            currentAccount = currentAccount.Deposit(amount);
            accountManager.UpdateAccount(currentAccount);
        }

        public void Withdrawal(double amount)
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }
            if (amount > GetCurrentBalance())
            {
                throw new ArgumentException($"Insufficient funds to withdraw given amount. Current balance={GetCurrentBalance()}, amount={amount}", "amount");
            }

            currentAccount = currentAccount.Withdrawal(amount);
            accountManager.UpdateAccount(currentAccount);
        }

        public double GetCurrentBalance()
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }

            return currentAccount.Balance;
        }

        public List<Transaction> GetTransactionHistory()
        {
            if (!IsLoggedIn())
            {
                throw new NullReferenceException("Current user is not set, must be logged in first");
            }

            return currentAccount.Transactions;
        }

        public bool IsLoggedIn()
        {
            return null != currentAccount;
        }

        public string GetCurrentUser()
        {
            return currentAccount?.Username;
        }

        public bool UserExists(string username)
        {
            return accountManager.UserExists(username);
        }

        public void LogOut()
        {
            currentAccount = null;
        }
    }
}
