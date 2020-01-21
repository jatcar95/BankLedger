using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Constants
{

    /// <summary>
    /// Represents actions users can make in their account
    /// </summary>
    public enum UserAction
    {
        None = 0,
        LogIn,
        CreateAccount,
        Deposit,
        Withdrawal,
        CheckBalance,
        ViewTransactions,
        LogOut,
        DeleteLedger
    }
}
