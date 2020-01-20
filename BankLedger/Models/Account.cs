using BankLedger.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    class Account
    {
        public string Username { get; private set; }

        public string HashedPassword { get; private set; }

        public double Balance { get; private set; }

        private List<Transaction> transactions;
        public List<Transaction> Transactions
        {
            get
            {
                return new List<Transaction>(transactions);
            }
        }

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

        public Account Deposit(double amount)
        {
            transactions.Add(new Transaction(UserAction.Deposit, $"Amount: {amount}, new balance: {Balance + amount}"));
            return new Account(Username, HashedPassword, Balance + amount, transactions);
        }

        public Account Withdrawal(double amount)
        {
            if (amount > Balance)
            {
                throw new ArgumentException("Insufficient balance to withdraw desired amount", "amount");
            }

            transactions.Add(new Transaction(UserAction.Withdrawal, $"Amount: {amount}, new balance: {Balance - amount}"));
            return new Account(Username, HashedPassword, Balance - amount, transactions);
        }
    }
}
