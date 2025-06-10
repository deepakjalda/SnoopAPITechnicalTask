using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.Models
{
    /// <summary>
    /// Represents a financial transaction associated with a customer account.
    ///
    /// Example:
    /// Returns a list of all transactions for the specified customer,
    /// optionally filtered by query parameters such as date range, status, or type.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Unique identifier for the transaction.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Signed transaction amount. Negative for debits, positive for credits.
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Currency code (e.g., "GBP").
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Name of the merchant. May be null.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// Date of the transaction in ISO 8601 format (e.g., "2025-06-06T00:00:00+00:00").
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Status of the transaction (e.g., "Pending" or "Booked").
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Type of transaction (e.g., "Debit" or "Credit").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Sub-type of the transaction (e.g., "Card Payment", "ATM Withdrawal").
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// Numeric category ID ranging from 1 to 20 (inclusive).
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Description of the transaction supplied by the merchant or provider.
        /// </summary>
        public string Description { get; set; }
    }

}
