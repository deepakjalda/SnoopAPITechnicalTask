using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.Core.API
{
    public interface IApiClient
    {
        /// <summary>
        /// Creates a new REST request with the specified endpoint and HTTP method.
        /// Adds default headers such as Content-Type application/json.
        /// </summary>
        /// <param name="endpoint">The API endpoint path.</param>
        /// <param name="method">The HTTP method (GET, POST, etc.).</param>
        /// <returns>A configured <see cref="RestRequest"/> instance.</returns>
        RestRequest CreateRequest(string endpoint, Method method);
    }
}
