using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Services;
using Microsoft.AspNetCore.Http;

namespace JsonApiDotNetCore.QueryStrings
{
    /// <summary>
    /// Reads and processes the various query string parameters for a HTTP request.
    /// </summary>
    public interface IQueryStringReader
    {
        /// <summary>
        /// Reads and processes the key/value pairs from the request query string.
        /// </summary>
        /// <param name="disableQueryStringAttribute">
        /// The <see cref="DisableQueryStringAttribute" /> if set on the controller that is targeted by the current request.
        /// </param>
        void ReadAll(DisableQueryStringAttribute disableQueryStringAttribute);

        // TODO: FAKE IMPLEMENTATION
        QuerySet Parse(QueryCollection queryCollection);
    }
}
