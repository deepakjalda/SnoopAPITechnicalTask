using NUnit.Framework.Interfaces;
using SnoopAPITechnicalTask.Core.API;
using SnoopAPITechnicalTask.Models;
using SnoopAPITechnicalTask.Services.Api;
using SnoopAPITechnicalTask.TestData;
using SnoopAPITechnicalTask.Utilities;

namespace SnoopAPITechnicalTask.Tests;

/// <summary>
/// Contains tests for the Include operation to verify that successful responses are returned, 
/// and that the response is correctly filtered based on the provided query parameters and maintain specfic filter order.
/// </summary>
public class SpecificOrderedTransactions
{
    #region SetUp and TearDown

    [SetUp]
    public async Task Setup()
    {
        m_baseUrl = TestContext.Parameters["BaseUrl"];
        m_endPoint = TestContext.Parameters["EndPoint"];
        m_apiClient = new TransactionApiClient(m_baseUrl, m_endPoint);
        m_schemaPath = Path.Combine(FileUtility.GetProjectRootPath(), "Schemas", "transaction-schema.json");
        m_customerTestData = await CustomerTestData.LoadFromFileAsync(Path.Combine(FileUtility.GetProjectRootPath(), "TestData", "customerTestData.json"));
        Hooks.Test = Hooks.Extent.CreateTest(TestContext.CurrentContext.Test.Name);

    }

    [TearDown]
    public void AfterEachTest()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status; // Passed, Failed, etc.
        var message = TestContext.CurrentContext.Result.Message;       // Any exception or message

        // Log the test result based on NUnit's outcome
        switch (status)
        {
            case TestStatus.Passed:
                Hooks.Test.Pass("Test passed.");
                break;

            case TestStatus.Failed:
                Hooks.Test.Fail("Test failed: " + message);
                break;

            case TestStatus.Skipped:
                Hooks.Test.Skip("Test skipped.");
                break;

            default:
                Hooks.Test.Warning("Test ended with an unknown status.");
                break;
        }
    }

    #endregion

    #region Tests

    [Test]
    [Description("Verify that all transactions with 'Pending' status are listed first.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsPendingStatusTransactionsAreFirst(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.IsNotNull(response);
        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data);
        Assert.IsNotEmpty(response.Data, "No transactions were returned.");

        bool encounteredNonPending = false;

        foreach (var transaction in response.Data)
        {
            if (transaction.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                Assert.IsFalse(encounteredNonPending, "Pending transaction found after non-pending transactions.");
            else
                encounteredNonPending = true;
        }
    }

    [Test]
    [Description("Verify that 'Pending' transactions are listed first and within each status transactions are ordered by timestamp descending.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsPendingFirstAndOrderedByTimestampDescending(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            CustomerId = customerId
        };

        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.IsNotNull(response);
        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data);
        Assert.IsNotEmpty(response.Data, "No transactions were returned.");

        bool encounteredBooked = false;
        DateTime? lastPendingTimestamp = null;
        DateTime? lastBookedTimestamp = null;

        foreach (var transaction in response.Data)
        {
            var status = transaction.Status?.ToLowerInvariant();
            var timestamp = transaction.Timestamp;

            Assert.IsNotNull(timestamp, "Transaction timestamp should not be null.");

            if (status == "pending")
            {
                // Pending transactions must come before any booked transaction
                Assert.IsFalse(encounteredBooked, "Pending transaction found after Booked transaction.");

                // Timestamp descending check for pending group
                if (lastPendingTimestamp.HasValue)
                {
                    Assert.GreaterOrEqual(lastPendingTimestamp.Value, timestamp,
                        "Pending transactions are not ordered by timestamp descending.");
                }
                lastPendingTimestamp = timestamp;
            }
            else if (status == "booked")
            {
                encounteredBooked = true;

                // Timestamp descending check for booked group
                if (lastBookedTimestamp.HasValue)
                {
                    Assert.GreaterOrEqual(lastBookedTimestamp.Value, timestamp,
                        "Booked transactions are not ordered by timestamp descending.");
                }
                lastBookedTimestamp = timestamp;
            }
            else
            {
                Assert.Fail($"Unexpected transaction status: {transaction.Status}");
            }
        }
    }


    [Test]
    [Description("Verify transaction ordering: Pending before Booked, ordered by timestamp descending, and by merchantName alphabetically for equal timestamps (nulls first need to confirm).")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsToVerifyTransactionOrderingRules(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            CustomerId = customerId
        };

        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        // Assert
        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data, "Response data is null.");
        Assert.IsNotEmpty(response.Data, "No transactions returned.");

        bool encounteredBooked = false;
        string? previousStatus = null;
        DateTimeOffset? lastTimestamp = null;
        string? lastMerchantName = null;

        foreach (var transaction in response.Data)
        {
            string status = transaction.Status?.ToLowerInvariant() ?? "";
            DateTimeOffset timestamp = DateTimeOffset.Parse(transaction.Timestamp.ToString()!);
            string? merchantName = transaction.MerchantName;

            // Check pending comes before Booked
            if (status == "pending")
            {
                Assert.IsFalse(encounteredBooked, "Found a Pending transaction after Booked ones.");
            }
            else if (status == "booked")
            {
                encounteredBooked = true;
            }
            else
            {
                Assert.Fail($"Unexpected transaction status: {transaction.Status}");
            }

            //  Within each status group, timestamps should be in descending order
            if (previousStatus == status && lastTimestamp.HasValue)
            {
                if (timestamp > lastTimestamp.Value)
                {
                    Assert.Fail("Timestamps are not in descending order.");
                }

                // If timestamps are equal, order by merchantName alphabetically, nulls FIRST { I need to confirm this}
                if (timestamp == lastTimestamp.Value)
                {
                    if (lastMerchantName != null && merchantName != null)
                    {
                        Assert.That(
                            string.Compare(lastMerchantName, merchantName, StringComparison.OrdinalIgnoreCase) <= 0,
                            "Merchant names are not in ascending alphabetical order for equal timestamps."
                        );
                    }
                    else if (lastMerchantName != null && merchantName == null)
                    {
                        Assert.Fail("Null merchant name should come before named merchant for same timestamp.");
                    }
                }
            }

            // Store current values for next iteration
            lastTimestamp = timestamp;
            lastMerchantName = merchantName;
            previousStatus = status;
        }
    }

#endregion

    #region Member Variables

    private CustomerTestData m_customerTestData;
    private TransactionApiClient m_apiClient;
    private string m_baseUrl;
    private string m_endPoint;
    private string m_schemaPath;

    #endregion

}
