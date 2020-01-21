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
    public class AccountManagerTests
    {
        private AccountManager manager;

        [TestInitialize]
        public void InitializeTest()
        {
            var hasher = new PasswordHasher<string>();
            manager = new AccountManager(hasher);
        }

        [TestCleanup]
        public void Cleanup()
        {
            manager = null;
        }

        [TestMethod]
        public void TestNullHasher()
        {
            Assert.ThrowsException<ArgumentNullException>(() => (new AccountManager(null)));
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            manager.CreateAccount("testUsername", "testPassword");
            Assert.IsTrue(manager.UserExists("testUsername"));
            var account = manager.LogIn("testUsername", "testPassword");
            Assert.AreEqual("testUsername", account.Username);
        }

        [TestMethod]
        public void TestLogin_NoUser()
        {
            var account = manager.LogIn("testUsername", "testPassword");
            Assert.IsNull(account);
        }

        [DataRow("incorrectPassword")]
        [DataRow("testpassword")]
        [DataRow("")]
        [DataRow("TESTPASSWORD")]
        [DataTestMethod]
        public void TestLogin_PasswordIncorrect(string password)
        {
            manager.CreateAccount("testUsername", "testPassword");
            Assert.IsTrue(manager.UserExists("testUsername"));
            var account = manager.LogIn("testUsername", password);
            Assert.IsNull(account);
        }

        [TestMethod]
        public void TestCreateAccount_SameUsername()
        {
            manager.CreateAccount("testUsername", "testPassword");
            Assert.ThrowsException<ArgumentException>(() => manager.CreateAccount("testUsername", "testPassword"));
        }

        [DataRow("", "password")]
        [DataRow("user", "")]
        [DataRow("", "")]
        [DataTestMethod]
        public void TestCreateAccount_EmptyFields(string username, string password)
        {
            manager.CreateAccount(username, password);
            Assert.IsTrue(manager.UserExists(username));
            var account = manager.LogIn(username, password);
            Assert.AreEqual(username, account.Username);
        }

        [DataRow(null, "")]
        [DataRow("", null)]
        [DataRow(null, null)]
        [DataTestMethod]
        public void TestCreateAccount_ThrowsException(string username, string password)
        {
            Assert.ThrowsException<ArgumentNullException>(() => manager.CreateAccount(username, password));
        }

        [TestMethod]
        public void TestLogin_Multiple()
        {
            for (var i = 0; i < 100; i++)
            {
                manager.CreateAccount("user" + i, "pass" + i);
                Assert.IsTrue(manager.UserExists("user" + i));
                var account = manager.LogIn("user" + i, "pass" + i);
                Assert.AreEqual("user" + i, account.Username);
            }
        }

        [TestMethod]
        public void TestUserExists_NotExists()
        {
            Assert.IsFalse(manager.UserExists("testUsername"));
            manager.CreateAccount("testUsername", "testPassword");
            Assert.IsFalse(manager.UserExists("anotherTestUsername"));
        }

        [TestMethod]
        public void TestUpdateAccount_AccountNotExists()
        {
            var account = new Account("testUsername", "hashedPassword");
            Assert.ThrowsException<ArgumentException>(() => manager.UpdateAccount(account));
        }

        [TestMethod]
        public void TestUpdateAccount_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => manager.UpdateAccount(null));
        }

        [TestMethod]
        public void TestUpdateAccount()
        {
            manager.CreateAccount("testUsername", "testPassword");
            var account = manager.LogIn("testUsername", "testPassword");
            var updatedAccount = account.Deposit(200);
            manager.UpdateAccount(updatedAccount);
            account = manager.LogIn("testUsername", "testPassword");
            Assert.AreEqual(200, account.Balance);
        }

        [TestMethod]
        public void TestUpdateAccount_Immutable()
        {
            manager.CreateAccount("testUsername", "testPassword");
            var account = manager.LogIn("testUsername", "testPassword");
            var updatedAccount = account.Deposit(20);
            Assert.AreNotEqual(updatedAccount, manager.LogIn("testUsername", "testPassword"));
        }
    }
}
