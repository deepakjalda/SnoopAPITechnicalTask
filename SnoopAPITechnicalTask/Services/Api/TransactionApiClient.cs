using RestSharp;
using SnoopAPITechnicalTask.Attributes;
using SnoopAPITechnicalTask.Core.API;
using SnoopAPITechnicalTask.Models;

namespace SnoopAPITechnicalTask.Services.Api;

/// <summary>
/// Client for interacting with the Transactions API endpoints.
/// Provides methods to retrieve and manage transaction data for customers.
/// </summary>
public class TransactionApiClient : BaseApiClient
{
    #region Constructor
    public TransactionApiClient(string baseUrl, string endpoint) : base(baseUrl) 
    {
        m_endPoint = endpoint;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Retrieves a list of transactions for a specified customer, filtered by the provided query parameters.
    /// </summary>
    /// <param name="queryParams">The parameters used to filter the transactions. Must include a valid <see cref="TransactionQueryParameters.CustomerId"/>.</param>
    /// <returns> <see cref="RestResponse{List{Transaction}}"/> containing the filtered list of transactions.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryParams"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <see cref="TransactionQueryParameters.CustomerId"/> is null or empty.</exception>
    public async Task<RestResponse<List<Transaction>>> GetTransactionsAsync(TransactionQueryParameters queryParams)
    {
        if (queryParams == null)
            throw new ArgumentNullException(nameof(queryParams));
      //  if (string.IsNullOrWhiteSpace(queryParams.CustomerId))
      //      throw new ArgumentException("CustomerId is required.", nameof(queryParams.CustomerId));

        var request = CreateRequest(m_endPoint, Method.Get);

        var properties = typeof(TransactionQueryParameters).GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(queryParams);
            if (value == null)
                continue;

            string stringValue = value is bool b ? b.ToString().ToLower() : value.ToString();
            if (string.IsNullOrWhiteSpace(stringValue))
                continue;

            var attr = prop.GetCustomAttributes(typeof(QueryParamAttribute), false)
                           .FirstOrDefault() as QueryParamAttribute;

            string paramName = attr?.Name ?? prop.Name;

            request.AddQueryParameter(paramName, stringValue);
        }

        return await _client.ExecuteAsync<List<Transaction>>(request);
    }

    /// <summary>
    /// Retrieves a list of transactions for a specified customer, filtered by the provided query parameters.
    /// </summary>
    /// <param name="customerId">The parameters used to filter the transactions. Must include a customerId.</param>
    /// <returns> <see cref="RestResponse{List{Transaction}}"/> containing the filtered list of transactions.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryParams"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <see cref="TransactionQueryParameters.CustomerId"/> is null or empty.</exception>
    public async Task<RestResponse<List<Transaction>>> GetTransactionsAsync(string customerId)
    {
        var queryParams = new TransactionQueryParameters
        {
            CustomerId = customerId
        };

        return await GetTransactionsAsync(queryParams);
    }

    #endregion

    #region Member variables

    private readonly string m_endPoint;

    #endregion

}