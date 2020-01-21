using BankLedger.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    /// <summary>
    /// Represents a user's account, and maintains various properties about it.
    /// Immutable.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Username associated with this account
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Hashed password associated with this account
        /// </summary>
        public string HashedPassword { get; private set; }

        /// <summary>
        /// This account's current balance
        /// </summary>
        public double Balance { get; private set; }

        private List<Transaction> transactions;
        /// <summary>
        /// The transaction history for this account
        /// </summary>
        public List<Transaction> Transactions
        {
            get
            {
                return new List<Transaction>(transactions);
            }
        }

        /// <summary>
        /// Creates a new account
        /// </summary>
        /// <param name="username">The username for the account to be created</param>
        /// <param name="hashedPassword">The hashed password for the account to be created. NOT THE RAW PASSWORD.</param>
        /// <param name="balance">The balance for the account to start with</param>
        /// <param name="transactions">The transaction history for this account to start with</param>
        public Account(string username, string hashedPassword, double balance = 0, List<Transaction> transactions = null)
        {
            Username = username;
            HashedPassword = hashedPassword;
            Balance = balance;
            this.transactions = new List<Transaction>();
            if (null != transactions)
            {
                this.transactions.AddRange(transactions);
            }
        }

        /// <summary>
        /// Returns a copy of this account with the given amount added to its balance
        /// </summary>
        /// <param name="amount">The amount to deposit</param>
        /// <returns>A new account identical to this account, but with the balance and transaction history 
        /// updated to reflect the deposit</returns>
        public Account Deposit(double amount)
        {
            return ChangeBalance(amount, UserAction.Deposit);
        }

        /// <summary>
        /// Returns a copy of this account with the given amount subtracted from its balance
        /// </summary>
        /// <param name="amount">The amount to withdraw</param>
        /// <returns>A new account identical to this account, but with the balance and transaction history 
        /// updated to reflect the withdrawal</returns>
        public Account Withdrawal(double amount)
        {
            return ChangeBalance(-amount, UserAction.Withdrawal);
        }

        private Account ChangeBalance(double amount, UserAction action)
        {
            transactions.Add(new Transaction(action, DateTimeOffset.Now, $"Amount: {Math.Abs(amount)}, new balance: {Balance + amount}"));
            return new Account(Username, HashedPassword, Balance + amount, transactions);
        }
    }
}
