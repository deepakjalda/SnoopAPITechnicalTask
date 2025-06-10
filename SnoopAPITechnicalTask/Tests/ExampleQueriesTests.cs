using NUnit.Framework.Interfaces;
using SnoopAPITechnicalTask;
using SnoopAPITechnicalTask.Core.API;
using SnoopAPITechnicalTask.Models;
using SnoopAPITechnicalTask.Services.Api;
using SnoopAPITechnicalTask.TestData;
using SnoopAPITechnicalTask.Utilities;

namespace SnoopAPITechnicalTask.Tests;

/// <summary>
/// Contains tests for the Include operation to verify that successful responses are returned, 
/// and that the response is correctly filtered based on the provided query parameters.
/// </summary>
public class ExampleQueriesTests
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
    [Description("Verify that no transaction has status 'Pending'.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsShouldNotContainPendingStatus(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            IncludePending = false,
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data, "Response data is null.");

        bool hasPending = response.Data.Any(t => string.Equals(t.Status, "Pending", StringComparison.OrdinalIgnoreCase));

        Assert.That(hasPending, Is.False, "Expected no transactions with status 'Pending', but found some.");
    }

    [Test]
    [Description("Verify that no transactions have status 'Pending' and no transactions have type 'Credit'.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsShouldNotContainPendingOrCredit(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            IncludePending = false,
            IncludeCredit = false,
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data, "Response data is null.");

        bool hasPending = response.Data.Any(t => string.Equals(t.Status, "Pending", StringComparison.OrdinalIgnoreCase));
        bool hasCredit = response.Data.Any(t => string.Equals(t.Type, "Credit", StringComparison.OrdinalIgnoreCase));

        Assert.That(hasCredit, Is.False, "Expected no transactions with type 'Credit', but found some.");
        Assert.That(hasPending, Is.False, "Expected no transactions with status 'Pending', but found some.");
    }

    [Test]
    [Description("Verify that all returned transactions have a categoryId equal to 11")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetAllTransactionsHaveCategoryId11(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            Category = 11,
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data, "Response data is null.");

        Assert.That(response.Data.All(t => t.CategoryId == 11), Is.True, "Not all transactions have categoryId = 11.");
    }

    [Test]
    [Description("Verify that all returned transactions have a timestamp on or after 2025-05-01.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsWithFromDateFilterReturnsValidTransaction(string customerId)
    {
        string fromDatevalue = "2025-06-06";

        var queryparams = new TransactionQueryParameters()
        {
            FromDate = "2025-06-06",
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.IsNotNull(response);
        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data);
        Assert.IsNotEmpty(response.Data, "No transactions were returned.");

        // Verify all transactions have timestamp >= fromDate
        var fromDate = DateOnly.Parse(fromDatevalue);
        foreach (var transaction in response.Data)
        {
            var timestamp = DateOnly.Parse(transaction.Timestamp.ToString("yyyy-MM-dd"));
            Assert.That(timestamp, Is.GreaterThanOrEqualTo(fromDate),
                $"Transaction {transaction.TransactionId} has timestamp before {fromDate:yyyy-MM-dd}");
        }
    }

    [Test]
    [Description("Verify that all returned transactions have timestamps within the specified date range.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsWithFromDateAndToDateFilter_ReturnsValidTransactions(string customerId)
    {
        string fromDatevalue = "2025-06-05", toDateValue = "2025-06-06";

        var queryparams = new TransactionQueryParameters()
        {
            FromDate = fromDatevalue,
            ToDate = toDateValue,
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.IsNotNull(response);
        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data);
        Assert.IsNotEmpty(response.Data, "No transactions were returned.");

        var fromDate = DateOnly.Parse(fromDatevalue);
        var toDate = DateOnly.Parse(toDateValue); 

        foreach (var transaction in response.Data)
        {
            var timestamp = DateOnly.Parse(transaction.Timestamp.ToString("yyyy-MM-dd"));
            Assert.That(timestamp, Is.GreaterThanOrEqualTo(fromDate),
            $"Transaction {transaction.TransactionId} timestamp is before the fromDate {fromDate:yyyy-MM-dd}");
            Assert.That(timestamp, Is.LessThanOrEqualTo(toDate),
            $"Transaction {transaction.TransactionId} timestamp is after the toDate {toDate:yyyy-MM-dd}");
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
