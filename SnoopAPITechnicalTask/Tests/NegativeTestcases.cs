using AventStack.ExtentReports.Gherkin.Model;
using NUnit.Framework.Interfaces;
using SnoopAPITechnicalTask;
using SnoopAPITechnicalTask.Core.API;
using SnoopAPITechnicalTask.Models;
using SnoopAPITechnicalTask.Services.Api;
using SnoopAPITechnicalTask.TestData;
using SnoopAPITechnicalTask.Utilities;
using System.Diagnostics.Metrics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnoopAPITechnicalTask.Tests;

/// <summary>
/// Contains tests for the Include operation to verify that successful responses are returned, 
/// and that the response is correctly filtered based on the provided query parameters.
/// </summary>
public class NegativeTestcases
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
    [Description("Verifies that a Bad Request response is returned when an invalid customer ID GUID format is provided.")]
    public async Task ReturnsBadRequestWhenCustomerIdIsInvalidGuid()
    {
        var queryparams = new TransactionQueryParameters()
        {
            CustomerId = "kkdkdfkmdfkjdkfgdfkghdfgkdhfgdddddddddddddddddddddddddd"
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"",""), Is.EqualTo("Invalid customerId guid format"));
    }

    [Test]
    [Description("Verifies that a Bad Request response is returned when an invalid customer ID(special characters) GUID format is provided.")]
    public async Task ReturnsBadRequestWhenCustomerIdIsInvalidGuidSpecialCharacters()
    {
        var queryparams = new TransactionQueryParameters()
        {
            CustomerId = "hahshdhahd%883@32/sadsd£"
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"", ""), Is.EqualTo("Invalid customerId guid format"));
    }

    [Test]
    [Description("Verifies that a Bad Request response is returned when an No customer ID is provided.")]
    public async Task ReturnsBadRequestWhenCustomerIdIsInvalidGuidNoCustomerIDCharacters()
    {
        var queryparams = new TransactionQueryParameters()
        {
            CustomerId = ""
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"", ""), Is.EqualTo("Missing customerId query parameter"));
    }

    [Test]
    [Description("Verifies that a Bad Request response is returned with Invalid From date")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.InvalidCustomerIds))]
    public async Task ReturnsBadRequestWithInvalidFromDate(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            FromDate = "2022",
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"", ""), Is.EqualTo("fromDate must be in YYYY-MM-DD format"));
    }

    [Test]
    [Description("Verifies that a Bad Request response is returned with Invalid From date")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.InvalidCustomerIds))]
    public async Task ReturnsBadRequestWithInvalidToDate(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            ToDate = "2022",
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"", ""), Is.EqualTo("toDate must be in YYYY-MM-DD format"));
    }

    [Test]
    [Description("Verifies that a Bad Request response is returned with Invalid categoryID Greater than 20")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.InvalidCustomerIds))]
    public async Task ReturnsBadRequestWithIsInvalidCategoryID(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            Category = 232323,
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"", ""), Is.EqualTo("categoryId must be an integer between 1 and 20"));
    }

    [Test]
    [Description("Verifies that a Bad Request response is returned with Invalid categoryID with less than zero")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.InvalidCustomerIds))]
    public async Task ReturnsBadRequestWithIsInvalidCategoryIDLessThanZero(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            Category = -33,
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"", ""), Is.EqualTo("categoryId must be an integer between 1 and 20"));
    }

    [Test]
    [Description("Verifies that a Bad Request response is returned with toDate before fromDate")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.InvalidCustomerIds))]
    public async Task ReturnsBadRequestWithToDateAndFromDate(string customerId)
    {
        var queryparams = new TransactionQueryParameters()
        {
            ToDate = "2025-06-05",
            FromDate = "2025-06-06",
            CustomerId = customerId
        };
        var response = await m_apiClient.GetTransactionsAsync(queryparams);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest), "Expected HTTP status code 400 Bad Request.");
        Assert.That(response.Content.Replace("\"", ""), Is.EqualTo("toDate must be after fromDate (or on the same day)"));
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
