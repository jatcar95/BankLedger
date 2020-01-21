using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankLedger.Models;
using System.Collections.Generic;
using BankLedger.Constants;

namespace BankLedgerTests.ModelTests
{
    [TestClass]
    public class AccountTests
    {
        private Account account;

        [TestInitialize]
        public void InitializeTest()
        {
            account = new Account("testUsername", "testHash", 100);
        }

        [TestCleanup]
        public void Cleanup()
        {
            account = null;
        }

        [TestMethod]
        public void TestAlterTransactions()
        {
            CollectionAssert.AreEqual(new List<Transaction>(), account.Transactions);
            account.Transactions.Add(new Transaction(UserAction.Deposit, "test deposit"));
            CollectionAssert.AreEqual(new List<Transaction>(), account.Transactions);

            var transactions = new List<Transaction>();
            transactions.Add(new Transaction(UserAction.Deposit, "test deposit"));
            account = new Account("", "", 0, transactions);
            transactions.Clear();
            CollectionAssert.AreNotEqual(transactions, account.Transactions);
        }

        [DataRow("", "", 0, null)]
        [DataRow(null, null, 0, null)]
        [DataRow("testUsername", "testPassword", 0, null)]
        [DataTestMethod]
        public void TestAccountConstructor(string username, string password, double balance, List<Transaction> transactions)
        {
            var account = new Account(username, password, balance, transactions);
            Assert.AreEqual(username, account.Username);
            Assert.AreEqual(password, account.HashedPassword);
            Assert.AreEqual(balance, account.Balance);
            if (null == transactions)
            {
                CollectionAssert.AreEqual(new List<Transaction>(), account.Transactions);
            }
            else
            {
                Assert.AreEqual(transactions.Count, account.Transactions.Count);
                for (var i = 0; i < transactions.Count; i++)
                {
                    Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                    Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
                }
            }
        }

        [TestMethod]
        public void TestAccountConstructor_Transactions()
        {
            var transactions = new List<Transaction>
            {
                new Transaction(UserAction.Deposit, "test deposit"),
                new Transaction(UserAction.Withdrawal, "test withdrawal")
            };
            account = new Account("testUsername", "testPassword", 0, transactions);
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }
        }

        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        [DataRow(1.1)]
        [DataRow(1.000000001)]
        [DataRow(.000000001)]
        [DataTestMethod]
        public void TestAccountDeposit(double amount)
        {
            var transactions = new List<Transaction>();
            account = account.Deposit(amount);
            Assert.AreEqual(100 + amount, account.Balance);
            transactions.Add(new Transaction(UserAction.Deposit, $"Amount: {amount}, new balance: {100 + amount}"));
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }
        }

        [TestMethod]
        public void TestAccountDeposit_Multiple()
        {
            var transactions = new List<Transaction>();
            account = account.Deposit(20);
            transactions.Add(new Transaction(UserAction.Deposit, $"Amount: {20}, new balance: {120}"));
            Assert.AreEqual(120, account.Balance);
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }

            account = account.Deposit(-20);
            transactions.Add(new Transaction(UserAction.Deposit, $"Amount: {20}, new balance: {100}"));
            Assert.AreEqual(100, account.Balance);
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }

            account = account.Deposit(1.00001);
            transactions.Add(new Transaction(UserAction.Deposit, $"Amount: {1.00001}, new balance: {101.00001}"));
            Assert.AreEqual(101.00001, account.Balance);
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }
        }

        [DataRow(0)]
        [DataRow(1)]
        [DataRow(99)]
        [DataRow(100)]
        [DataRow(99.99999)]
        [DataRow(.000000001)]
        [DataRow(150)]
        [DataTestMethod]
        public void TestAccountWithdrawal(double amount)
        {
            var transactions = new List<Transaction>();
            account = account.Withdrawal(amount);
            Assert.AreEqual(100 - amount, account.Balance);
            transactions.Add(new Transaction(UserAction.Withdrawal, $"Amount: {amount}, new balance: {100 - amount}"));
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }
        }

        [TestMethod]
        public void TestWithdrawal_Multiple()
        {
            var transactions = new List<Transaction>();
            account = account.Withdrawal(20);
            transactions.Add(new Transaction(UserAction.Withdrawal, $"Amount: {20}, new balance: {80}"));
            Assert.AreEqual(80, account.Balance);
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }

            account = account.Withdrawal(-20);
            transactions.Add(new Transaction(UserAction.Withdrawal, $"Amount: {20}, new balance: {100}"));
            Assert.AreEqual(100, account.Balance);
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }

            account = account.Withdrawal(1.00001);
            transactions.Add(new Transaction(UserAction.Withdrawal, $"Amount: {1.00001}, new balance: {98.99999}"));
            Assert.AreEqual(98.99999, account.Balance);
            Assert.AreEqual(transactions.Count, account.Transactions.Count);
            for (var i = 0; i < transactions.Count; i++)
            {
                Assert.AreEqual(transactions[i].Action, account.Transactions[i].Action);
                Assert.AreEqual(transactions[i].Description, account.Transactions[i].Description);
            }
        }
    }
}
