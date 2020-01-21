using BankLedger.Models;
using BankLedger.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedgerTests.ServiceTests
{
    [TestClass]
    public class BankLedgerTests
    {
        private BankLedger.Services.BankLedger ledger;

        [TestInitialize]
        public void InitializeTest()
        {
            var hasher = new PasswordHasher<string>();
            var manager = new AccountManager(hasher);
            ledger = new BankLedger.Services.BankLedger(manager);
        }

        [TestCleanup]
        public void Cleanup()
        {
            ledger = null;
        }

        [TestMethod]
        public void TestLogIn_NoAccount()
        {
            Assert.IsFalse(ledger.LogIn("testUsername", "testPassword"));
            Assert.IsNull(ledger.GetCurrentUser());
            Assert.IsFalse(ledger.IsLoggedIn());
        }

        [TestMethod]
        public void TestLogIn()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            Assert.IsTrue(ledger.LogIn("testUsername", "testPassword"));
            Assert.AreEqual("testUsername", ledger.GetCurrentUser());
            Assert.IsTrue(ledger.IsLoggedIn());
        }

        [TestMethod]
        public void TestLogIn_IncorrectPassword()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            Assert.IsFalse(ledger.LogIn("testUsername", "incorrectPassword"));
            Assert.IsNull(ledger.GetCurrentUser());
            Assert.IsFalse(ledger.IsLoggedIn());
        }

        [TestMethod]
        public void TestLogIn_MultipleTimes()
        {
            ledger.CreateAccount("testUsername", "testPassword");

            Assert.IsTrue(ledger.LogIn("testUsername", "testPassword"));
            Assert.AreEqual("testUsername", ledger.GetCurrentUser());
            Assert.IsTrue(ledger.IsLoggedIn());

            Assert.IsTrue(ledger.LogIn("testUsername", "testPassword"));
            Assert.AreEqual("testUsername", ledger.GetCurrentUser());
            Assert.IsTrue(ledger.IsLoggedIn());

            Assert.IsTrue(ledger.LogIn("testUsername", "testPassword"));
            Assert.AreEqual("testUsername", ledger.GetCurrentUser());
            Assert.IsTrue(ledger.IsLoggedIn());
        }

        [TestMethod]
        public void TestDeposit_LoggedOut()
        {
            Assert.ThrowsException<NullReferenceException>(() => ledger.Deposit(0));
        }

        [TestMethod]
        public void TestDeposit()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            Assert.AreEqual(0, ledger.GetCurrentBalance());
            ledger.Deposit(100);
            Assert.AreEqual(100, ledger.GetCurrentBalance());
        }

        [TestMethod]
        public void TestDeposit_NegativeAmount()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            Assert.AreEqual(0, ledger.GetCurrentBalance());
            Assert.ThrowsException<ArgumentException>(() => ledger.Deposit(-100));
        }

        [TestMethod]
        public void TestWithdrawal_LoggedOut()
        {
            Assert.ThrowsException<NullReferenceException>(() => ledger.Withdrawal(0));
        }

        [TestMethod]
        public void TestWithdrawal()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            Assert.AreEqual(0, ledger.GetCurrentBalance());
            ledger.Deposit(100);
            Assert.AreEqual(100, ledger.GetCurrentBalance());
            ledger.Withdrawal(50);
            Assert.AreEqual(50, ledger.GetCurrentBalance());
        }

        [TestMethod]
        public void TestWithdrawal_NegativeAmount()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            Assert.AreEqual(0, ledger.GetCurrentBalance());
            Assert.ThrowsException<ArgumentException>(() => ledger.Withdrawal(-100));
        }

        [TestMethod]
        public void TestWithdrawal_InsufficientFunds()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            Assert.ThrowsException<ArgumentException>(() => ledger.Withdrawal(100));
            ledger.Deposit(100);
            Assert.ThrowsException<ArgumentException>(() => ledger.Withdrawal(101));
            ledger.Withdrawal(100);
            Assert.ThrowsException<ArgumentException>(() => ledger.Withdrawal(1));
        }

        [TestMethod]
        public void TestTransactions_LoggedOut()
        {
            Assert.ThrowsException<NullReferenceException>(() => ledger.GetTransactionHistory());
        }

        [TestMethod]
        public void TestTransactions_NoHistory()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            CollectionAssert.AreEqual(new List<Transaction>(), ledger.GetTransactionHistory());
        }

        [TestMethod]
        public void TestTransactions_Single()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            ledger.Deposit(100);
            Assert.AreEqual(1, ledger.GetTransactionHistory().Count);
        }

        [TestMethod]
        public void TestTransactions_Multiple()
        {
            ledger.CreateAccount("testUsername", "testPassword");
            ledger.LogIn("testUsername", "testPassword");
            ledger.Deposit(100);
            ledger.Deposit(200);
            ledger.Withdrawal(1.5);
            ledger.Withdrawal(145.3);
            Assert.AreEqual(4, ledger.GetTransactionHistory().Count);
        }

        [TestMethod]
        public void TestCurrentUser_LoggedOut()
        {
            Assert.IsNull(ledger.GetCurrentUser());
        }

        [TestMethod]
        public void TestUserExists_NotExists()
        {
            Assert.IsFalse(ledger.UserExists("testUser"));
        }

        [DataRow("testUser")]
        [DataRow("")]
        [DataTestMethod]
        public void TestUserExists(string username)
        {
            ledger.CreateAccount(username, "testPassword");
            Assert.IsTrue(ledger.UserExists(username));
        }

        [TestMethod]
        public void TestUserExists_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ledger.UserExists(null));
        }

        [TestMethod]
        public void TestLogOut_LoggedOut()
        {
            ledger.LogOut();
            Assert.IsNull(ledger.GetCurrentUser());
            Assert.IsFalse(ledger.IsLoggedIn());
        }

        [TestMethod]
        public void TestLogOut()
        {
            ledger.CreateAccount("testUser", "testPass");
            ledger.LogIn("testUser", "testPass");
            Assert.IsTrue(ledger.IsLoggedIn());
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
        }

        [TestMethod]
        public void TestLogOut_Multiple()
        {
            ledger.CreateAccount("testUser", "testPass");
            ledger.LogIn("testUser", "testPass");
            Assert.IsTrue(ledger.IsLoggedIn());
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
        }

        [TestMethod]
        public void TestLogOut_LogBackIn()
        {
            ledger.CreateAccount("testUser", "testPass");
            ledger.LogIn("testUser", "testPass");
            Assert.IsTrue(ledger.IsLoggedIn());
            ledger.Deposit(100);
            ledger.Withdrawal(20);
            ledger.LogOut();
            Assert.IsFalse(ledger.IsLoggedIn());
            ledger.LogIn("testUser", "testPass");
            Assert.AreEqual(80, ledger.GetCurrentBalance());
            Assert.AreEqual(2, ledger.GetTransactionHistory().Count);
        }
    }
}
