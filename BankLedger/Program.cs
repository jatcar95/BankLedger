using BankLedger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BankLedger
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                PrintWelcome();
                var option = GetValidUserInput();
                var serviceProvider = ConfigureServices();
                var ledger = serviceProvider.GetService<Models.BankLedger>();

                switch (option)
                {
                    case 1:
                        Login(ledger);
                        break;
                    case 2:
                        CreateAccount();
                        break;
                    default:
                        throw new ArgumentException("Invalid option parsed");
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

        private static void PrintWelcome()
        {
            Console.WriteLine("Welcome to AltSource Banking. Please choose one of the following options:");
            PrintOptions();
        }

        private static void PrintError()
        {
            Console.WriteLine("Invalid option selected. Please choose one of the following options:");
            PrintOptions();
        }

        private static void PrintOptions()
        {
            Console.WriteLine("\"1\": Login");
            Console.WriteLine("\"2\": Create a new account");
        }

        private static int GetValidUserInput()
        {
            var option = 0;
            while (!Int32.TryParse(Console.ReadLine(), out option) || (1 != option && 2 != option))
            {
                PrintError();               
            }
            return option;
        }

        private static bool Login(Models.BankLedger ledger)
        {
            // initial prompt to log in
            Console.WriteLine();
            Console.WriteLine("Please enter your username (or \"exit\" to return to the main menu):");
            var username = Console.ReadLine();
            if (username.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Returning to main menu...");
                return false;
            }
            Console.WriteLine("Please enter your password:");
            var password = Console.ReadLine();
            var loggedIn = ledger.LogIn(username, password);

            // verifying account exists and correct password
            while (!loggedIn)
            {
                Console.WriteLine("Invalid username/password combination, or account does not exist. Please try again.");
                Console.WriteLine("If you need to create an account, type \"exit\" to return to the main menu.");
                Console.WriteLine("Please enter your username (or \"exit\" to return to the main menu):");
                username = Console.ReadLine();
                if (username.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Returning to main menu...");
                    return false;
                }
                Console.WriteLine("Please enter your password:");
                password = Console.ReadLine();
                loggedIn = ledger.LogIn(username, password);
            }
            return true;
        }

        private static void CreateAccount()
        {
            Console.WriteLine("Created account :D:D:D:D");
        }
    }
}
