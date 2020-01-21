using BankLedger.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankLedger.Models
{
    /// <summary>
    /// Represents a transaction made on an account
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Creates a new Transaction object with the given parameters
        /// </summary>
        /// <param name="action">The action that this Transaction represents. Usually deposit or withdrawal.</param>
        /// <param name="time">The time this Transaction took place</param>
        /// <param name="description">Optional description of this transaction.</param>
        public Transaction(UserAction action, DateTimeOffset time, string description = null)
        {
            Action = action;
            Description = description;
            Time = time;
        }

        /// <summary>
        /// The action this Transaction represents
        /// </summary>
        public UserAction Action { get; private set; }

        /// <summary>
        /// Description of the Transaction
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Time the Transaction took place
        /// </summary>
        public DateTimeOffset Time { get; private set; }
    }
}
