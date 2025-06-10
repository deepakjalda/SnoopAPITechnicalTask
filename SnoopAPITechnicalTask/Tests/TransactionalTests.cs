using NUnit.Framework.Interfaces;
using SnoopAPITechnicalTask.Core.API;
using SnoopAPITechnicalTask.Models;
using SnoopAPITechnicalTask.Services.Api;
using SnoopAPITechnicalTask.TestData;
using SnoopAPITechnicalTask.Utilities;
using System.Net;

namespace SnoopAPITechnicalTask.Tests;

/// <summary>
/// Contains tests for the Include operation to verify successful responses. 
/// Ensures that, when transaction data is present, it matches the expected JSON schema 
/// for both expected and unexpected scenarios. Also checks that the error response is empty 
/// when no transactions are returned.
/// 
/// JSON schema file: <c>/Schemas/transaction-schema.json</c>
/// </summary>

public class TransactionalTests
{
    #region SetUp and TearDown

    [SetUp]
    public async Task Setup()
    {
        m_baseUrl = TestContext.Parameters["BaseUrl"];
        m_endPoint = TestContext.Parameters["EndPoint"];
        m_apiClient = new TransactionApiClient(m_baseUrl, m_endPoint);
        m_schemaPath = Path.Combine(FileUtility.GetProjectRootPath(), "Schemas", "transaction-schema.json");
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
    [Description("Verify Success status code is returned and response matches Json Schema for valid customer ID's")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.ValidCustomerIds))]
    public async Task GetTransactionsWithValidCustomerIdReturnsTransactionsWithExpectedBehaviour(string customerId)
    {
        var response = await m_apiClient.GetTransactionsAsync(customerId);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected HTTP status code 200 OK.");
        Assert.IsNotNull(response, $"Response was null for customerId: {customerId}");
        var errors = await JsonValidator.ValidateJsonFileAsync(response.Content,m_schemaPath);
        Assert.That(errors, Is.Empty, "JSON Schema validation failed:\n" + string.Join("\n", errors));
    }

    [Test]
    [Description("Verify Success status code is returned and response matches Json Schema for customer's which has transactions with unexpected behaviours.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.InvalidCustomerIds))]
    public async Task GetTransactionsWithInValidCustomerIdReturnsTransactionsWithUnexpectedBehaviour(string customerId)
    {
        var response = await m_apiClient.GetTransactionsAsync(customerId);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK), "Expected HTTP status code 200 OK.");
        Assert.IsNotNull(response, $"Response was null for customerId: {customerId}");
        var errors = await JsonValidator.ValidateJsonFileAsync(response.Content, m_schemaPath);
        Assert.That(errors, Is.Empty, "JSON validation failed:\n" + string.Join("\n", errors));
        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
    }

    [Test]
    [Description("Verify Success status code is returned and empty response is returned for customers with no transactions.")]
    [TestCaseSource(typeof(CustomerTestData), nameof(CustomerTestData.NoDataCustomerIds))]
    public async Task GetTransactionsWithValidCustomerIdAndReturnsNoTransactions(string customerId)
    {
        var response = await m_apiClient.GetTransactionsAsync(customerId);

        Assert.IsNotNull(response);
        Assert.That(response.IsSuccessful, Is.True, "API call failed.");
        Assert.IsNotNull(response.Data);
        Assert.IsEmpty(response.Data, "Expected no transactions but some were returned.");
    }

    #endregion

    #region Member Variables

    private TransactionApiClient m_apiClient;
    private string m_baseUrl;
    private string m_endPoint;
    private string m_schemaPath;

    #endregion

}
