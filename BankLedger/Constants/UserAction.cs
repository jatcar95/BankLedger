using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Constants
{
    enum UserAction
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
