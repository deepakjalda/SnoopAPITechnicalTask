using SnoopAPITechnicalTask.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.Models
{
    /// <summary>
    /// Represents the query parameters used to filter and retrieve transactions
    /// for a specific customer from the Transactions API.
    /// </summary>
    public class TransactionQueryParameters
    {
        /// <summary>
        /// Id of the customer (required).
        /// </summary>
        [QueryParam("customerId")]
        public string CustomerId { get; set; }

        /// <summary>
        /// If defined, return only transactions of that category.
        /// </summary>
        [QueryParam("categoryId")]
        public int? Category { get; set; }

        /// <summary>
        /// If true, includes transactions with "Pending" status. Default is true.
        /// </summary>
        [QueryParam("includePending")]
        public bool IncludePending { get; set; } = true;

        /// <summary>
        /// If true, includes transactions of "Debit" type. Default is true.
        /// </summary>
        [QueryParam("includeDebit")]
        public bool IncludeDebit { get; set; } = true;

        /// <summary>
        /// If true, includes transactions of "Credit" type. Default is true.
        /// </summary>
        [QueryParam("includeCredit")]
        public bool IncludeCredit { get; set; } = true;

        /// <summary>
        /// Include transactions from this date (inclusive) in YYYY-MM-DD format.
        /// </summary>
        [QueryParam("fromDate")]
        public string FromDate { get; set; }

        /// <summary>
        /// Include transactions up to this date (inclusive) in YYYY-MM-DD format.
        /// </summary>
        [QueryParam("toDate")]
        public string ToDate { get; set; }
    }

}
