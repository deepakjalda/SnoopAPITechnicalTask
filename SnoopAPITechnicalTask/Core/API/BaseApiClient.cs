using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.Core.API
{
    using RestSharp;
    using System;

    /// <summary>
    /// Base class for REST API clients using RestSharp.
    /// Provides common setup and helper methods for sending HTTP requests.
    /// </summary>
    public abstract class BaseApiClient : IApiClient
    {
        /// <summary>
        /// The RestSharp client used to send HTTP requests.
        /// </summary>
        protected readonly RestClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiClient"/> class with the specified base URL.
        /// </summary>
        /// <param name="baseUrl">The base URL of the API.</param>
        protected BaseApiClient(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be null or empty.", nameof(baseUrl));

            var options = new RestClientOptions(baseUrl)
            {
                ThrowOnAnyError = false,
                Timeout = TimeSpan.FromMilliseconds(10000)                // Timeout in milliseconds (10 seconds)
            };

            _client = new RestClient(options);
        }

        /// <summary>
        /// Creates a new REST request with the specified endpoint and HTTP method.
        /// Adds default headers such as Content-Type application/json.
        /// </summary>
        /// <param name="endpoint">The API endpoint path, relative to the base URL.</param>
        /// <param name="method">The HTTP method (GET, POST, etc.).</param>
        /// <returns>A configured <see cref="RestRequest"/> instance.</returns>
        public RestRequest CreateRequest(string endpoint, Method method)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be null or empty.", nameof(endpoint));

            var request = new RestRequest(endpoint, method);
            request.AddHeader("Content-Type", "application/json");
            return request;
        }
    }
}
