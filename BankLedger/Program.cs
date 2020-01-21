using BankLedger.Constants;
using BankLedger.Models;
using BankLedger.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BankLedger
{
    class Program
    {
        // Keyword for users to type to return to a previous menu
        public const string EXIT_KEYWORD = "exit";

        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var ledger = serviceProvider.GetService<Services.BankLedger>();
            // since user data is not persisted, you will have to force-shut-down the system
            // to stop it from running
            while (true)
            {
                var option = GetInitialUserAction();

                while (option != UserAction.LogIn)
                {
                    CreateAccount(ledger);
                    option = GetInitialUserAction();
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

        /// <summary>
        /// Configures services for dependency injection
        /// </summary>
        /// <returns>ServiceProvider containing the necessary services
        /// to run this program</returns>
        private static ServiceProvider ConfigureServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<Services.BankLedger>()
                .AddSingleton<PasswordHasher<string>>()
                .AddSingleton<AccountManager>()
                .BuildServiceProvider();
            return serviceProvider;
        }

        /// <summary>
        /// Gets the user's initial action from stdin upon starting the program: either loggin in,
        /// or creating an account.
        /// </summary>
        /// <returns>The action the user wishes to take</returns>
        private static UserAction GetInitialUserAction()
        {
            PrintWelcome();
            var option = 0;
            while (!int.TryParse(Console.ReadLine(), out option) || !(1 == option || 2 == option))
            {
                PrintError();               
            }
            return (UserAction)option;
        }

        /// <summary>
        /// Gets an action from stdin that the user wishes to perform on
        /// their account.
        /// </summary>
        /// <param name="ledger">The BankLedger used to manage user accounts</param>
        /// <returns>The action the user wishes to take</returns>
        private static UserAction GetUserAccountAction(Services.BankLedger ledger)
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

        // Logs in the user using the given BankLedger by prompting for their username
        // and password until a successful match is found (or the user decides to exit).
        private static void LogIn(Services.BankLedger ledger)
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
            // returns without logging in if the user types the exit keyword
            if (username.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Returning to main menu...");
                Console.WriteLine();
            }
        }

        // Prompts the user from stdin to create a new account
        private static void CreateAccount(Services.BankLedger ledger)
        {
            Console.WriteLine();
            Console.WriteLine($"Enter your desired username (or \"{EXIT_KEYWORD}\" to return to the main menu):");
            var username = Console.ReadLine();
            // verifying account doesn't already exist with given username
            while (!username.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase) && ledger.UserExists(username))
            {
                Console.WriteLine($"Username {username} is already taken. Please enter a different username:");
                username = Console.ReadLine();
            }
            Console.WriteLine();
            if (!username.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Enter your desired password:");
                var password = Console.ReadLine();
                Console.WriteLine("Re-enter your password:");
                // verifying user entered the same password twice
                while (!password.Equals(Console.ReadLine()))
                {
                    Console.WriteLine();
                    Console.WriteLine("The passwords you entered did not match. Please try again.");
                    Console.WriteLine("Enter your desired password:");
                    password = Console.ReadLine();
                    Console.WriteLine("Re-enter your password:");
                }

                // creating the account
                ledger.CreateAccount(username, password);
                Console.WriteLine();
                Console.WriteLine("Account successfully created! Please log in.");
                Console.WriteLine();
            }
        }

        // Prompts the user from stdin to record a deposit on their account
        private static void Deposit(Services.BankLedger ledger)
        {
            Console.WriteLine("Enter amount to deposit:");
            Console.WriteLine($"Type {EXIT_KEYWORD} to return to menu.");
            Console.Write("$");
            var amount = 0.0;
            var input = Console.ReadLine();
            // verifying user typed valid input (can be parsed as double and > 0, or exit keyword)
            while (!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase) && !(double.TryParse(input, out amount) && amount >= 0))
            {
                Console.WriteLine();
                Console.WriteLine("Invalid amount entered.");
                Console.WriteLine($"Enter an amount to deposit, or type {EXIT_KEYWORD} to return to menu:");
                Console.Write("$");
                input = Console.ReadLine();
            }

            // don't deposit if they typed exit keyword
            if (!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                ledger.Deposit(amount);
                Console.WriteLine();
                Console.WriteLine($"Deposit successful. New account balance: {ledger.GetCurrentBalance()}");
            }
            Console.WriteLine();
        }

        // Prompts the user from stdin to record a withdrawal from their account
        private static void Withdrawal(Services.BankLedger ledger)
        {
            Console.WriteLine("Enter amount to withdraw:");
            Console.WriteLine($"Type {EXIT_KEYWORD} to return to menu.");
            Console.Write("$");
            var amount = -1.0;
            var input = Console.ReadLine();
            var currentBalance = ledger.GetCurrentBalance();
            // TODO add regex for $xx.xx
            //verifying user typed valid input (can be parsed as double and > 0, or exit keyword)
            while (!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase) && !(double.TryParse(input, out amount) && amount <= currentBalance && amount >= 0))
            {
                Console.WriteLine();
                if (amount > 0)
                {
                    // user entered amount greater than their balance. Reset amount to -1 to continue loop
                    Console.WriteLine($"Insufficient funds to withdraw ${amount}");
                    amount = -1.0;
                }
                else
                {
                    Console.WriteLine("Invalid amount entered.");
                }
                Console.WriteLine($"Enter an amount to withdraw, or type {EXIT_KEYWORD} to return to menu:");
                Console.Write("$");
                input = Console.ReadLine();
            }

            // don't withdraw if they typed the exit keyword
            if (!input.Equals(EXIT_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                ledger.Withdrawal(amount);
                Console.WriteLine();
                Console.WriteLine($"Withdrawal successful. New account balance: {ledger.GetCurrentBalance()}");
            }
            Console.WriteLine();
        }

        // Prints the current user's balance to stdout
        private static void CheckBalance(Services.BankLedger ledger)
        {
            Console.WriteLine($"Current account balance: {ledger.GetCurrentBalance()}");
            Console.WriteLine();
        }

        // Prints the current users's transaction history to stdout
        private static void PrintTransactions(Services.BankLedger ledger)
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

        // Logs out the current user from the system
        private static void LogOut(Services.BankLedger ledger)
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

        private static void PrintUserWelcome(Services.BankLedger ledger)
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
