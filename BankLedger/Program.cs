using BankLedger.Constants;
using BankLedger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BankLedger
{
    class Program
    {
        public const string EXIT_KEYWORD = "exit";

        static void Main(string[] args)
        {
            // TODO implement exit
            bool exit = false;
            var serviceProvider = ConfigureServices();
            var ledger = serviceProvider.GetService<Models.BankLedger>();
            while (!exit)
            {
                var option = GetInitialUserAction();
                
                if (UserAction.CreateAccount == option)
                {
                    CreateAccount(ledger);
                }

                LogIn(ledger);
                while (ledger.IsLoggedIn())
                {
                    option = GetUserAccountAction(ledger);
                    switch (option)
                    {
                        case UserAction.Deposit:
                            Deposit(ledger);
                            break;
                        case UserAction.Withdrawal:
                            Withdrawal(ledger);
                            break;
                        case UserAction.CheckBalance:
                            CheckBalance(ledger);
                            break;
                        case UserAction.ViewTransactions:
                            PrintTransactions(ledger);
                            break;
                        case UserAction.LogOut:
                            LogOut(ledger);
                            break;
                        default:
                            throw new ArgumentException("Invalid option selected");
                    }
                }
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<Models.BankLedger>()
                .AddSingleton<PasswordHasher<string>>()
                .AddSingleton<AccountManager>()
                .BuildServiceProvider();
            return serviceProvider;
        }

        private static UserAction GetInitialUserAction()
        {
            PrintWelcome();
            var option = 0;
            while (!Int32.TryParse(Console.ReadLine(), out option) || !(1 == option || 2 == option))
            {
                PrintError();               
            }
            return (UserAction)option;
        }

        private static UserAction GetUserAccountAction(Models.BankLedger ledger)
        {
            PrintUserWelcome(ledger);
            var option = 0;
            while (!Int32.TryParse(Console.ReadLine(), out option) || option < 1 || option > 5)
            {
                PrintUserError();
            }
            Console.WriteLine();
            // adding two to ignore first to UserActions, LogIn and CreateAccount
            return (UserAction)(option + 2);
        }

        private static void LogIn(Models.BankLedger ledger)
        {
            // initial prompt to log in
            Console.WriteLine();
            Console.WriteLine($"Enter your username (or \"{EXIT_KEYWORD}\" to return to the main menu):");
            var username = Console.ReadLine();
            if (!username.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Enter your password:");
                var password = Console.ReadLine();
                var loggedIn = ledger.LogIn(username, password);

                // verifying account exists and correct password
                while (!username.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase) && !loggedIn)
                {
                    Console.WriteLine("Invalid username/password combination, or account does not exist. Please try again.");
                    Console.WriteLine($"If you need to create an account, type \"{EXIT_KEYWORD}\" to return to the main menu.");
                    Console.WriteLine();
                    Console.WriteLine("Enter your username:");
                    username = Console.ReadLine();
                    if (!username.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Enter your password:");
                        password = Console.ReadLine();
                        loggedIn = ledger.LogIn(username, password);
                    }
                }
                Console.WriteLine();
            }
            if (username.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Returning to main menu...");
                Console.WriteLine();
            }
        }

        private static void CreateAccount(Models.BankLedger ledger)
        {
            // TODO handle 'exit' keyword
            Console.WriteLine();
            Console.WriteLine("Enter your desired username:");
            var username = Console.ReadLine();
            while (ledger.UserExists(username))
            {
                Console.WriteLine($"Username {username} is already taken. Please enter a different username:");
                username = Console.ReadLine();
            }
            Console.WriteLine();
            Console.WriteLine("Enter your desired password:");
            var password = Console.ReadLine();
            Console.WriteLine("Re-enter your password:");
            while (!password.Equals(Console.ReadLine()))
            {
                Console.WriteLine();
                Console.WriteLine("The passwords you entered did not match. Please try again.");
                Console.WriteLine("Enter your desired password:");
                password = Console.ReadLine();
                Console.WriteLine("Re-enter your password:");
            }

            ledger.CreateAccount(username, password);
            Console.WriteLine();
            Console.WriteLine("Account creation successful! Redirecting to login.");
        }

        private static void Deposit(Models.BankLedger ledger)
        {
            Console.WriteLine("Enter amount to deposit:");
            Console.WriteLine($"Type {EXIT_KEYWORD} to return to menu.");
            Console.Write("$\t");
            var amount = 0.0;
            var input = Console.ReadLine();
            while (!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase) && !Double.TryParse(input, out amount))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid amount entered.");
                Console.WriteLine($"Enter an amount to deposit, or type {EXIT_KEYWORD} to return to menu:");
                input = Console.ReadLine();
            }

            if (!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                ledger.Deposit(amount);
                Console.WriteLine();
                Console.WriteLine($"Deposit successful. New account balance: {ledger.GetCurrentBalance()}");
            }
            Console.WriteLine();
        }

        private static void Withdrawal(Models.BankLedger ledger)
        {
            Console.WriteLine("Enter amount to withdraw:");
            Console.WriteLine($"Type {EXIT_KEYWORD} to return to menu.");
            Console.Write("$\t");
            var amount = -1.0;
            var input = Console.ReadLine();
            var currentBalance = ledger.GetCurrentBalance();
            // TODO add regex for $xx.xx
            while ((!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase) && !Double.TryParse(input, out amount)) || amount > currentBalance)
            {
                Console.WriteLine();
                if (amount > 0)
                {
                    Console.WriteLine($"Insufficient funds to withdraw ${amount}");
                    amount = -1.0;
                }
                else
                {
                    Console.WriteLine("Invalid amount entered.");
                }
                Console.WriteLine($"Enter an amount to withdraw, or type {EXIT_KEYWORD} to return to menu:");
                input = Console.ReadLine();
            }

            if (!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                ledger.Withdrawal(amount);
                Console.WriteLine();
                Console.WriteLine($"Withdrawal successful. New account balance: {ledger.GetCurrentBalance()}");
            }
            Console.WriteLine();
        }

        private static void CheckBalance(Models.BankLedger ledger)
        {
            Console.WriteLine($"Current account balance: {ledger.GetCurrentBalance()}");
            Console.WriteLine();
        }

        private static void PrintTransactions(Models.BankLedger ledger)
        {
            Console.WriteLine("Transaction history:");
            var transactions = ledger.GetTransactionHistory();
            foreach (var transaction in transactions)
            {
                Console.WriteLine($"{transaction.Time.ToLocalTime()}: {transaction.Action}");
                if (null != transaction.Description)
                {
                    Console.WriteLine($"\t{transaction.Description}");
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }

        private static void LogOut(Models.BankLedger ledger)
        {
            Console.WriteLine("Logging out...");
            Console.WriteLine();
            ledger.LogOut();
        }

        private static void PrintWelcome()
        {
            Console.WriteLine("Welcome to AltSource Banking. Please choose one of the following options:");
            PrintOptions();
        }

        private static void PrintError()
        {
            Console.WriteLine();
            Console.WriteLine("Invalid option selected. Please choose one of the following options:");
            PrintOptions();
        }

        private static void PrintOptions()
        {
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Create a new account");
        }

        private static void PrintUserWelcome(Models.BankLedger ledger)
        {
            Console.WriteLine($"Welcome {ledger.GetCurrentUser()}! Please choose one of the following options:");
            PrintAccountOptions();
        }

        private static void PrintUserError()
        {
            Console.WriteLine();
            Console.WriteLine("Invalid option selected. Please choose one of the following options:");
            PrintAccountOptions();
        }

        private static void PrintAccountOptions()
        {
            Console.WriteLine("1: Make a deposit");
            Console.WriteLine("2: Make a withdrawal");
            Console.WriteLine("3: Check current balance");
            Console.WriteLine("4: View transaction history");
            Console.WriteLine("5: Log out");
        }
    }
}
