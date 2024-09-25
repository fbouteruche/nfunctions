using Microsoft.AspNetCore.Http;
using NFunctions.Http;

namespace NFunctions.Azure
{
    public class AzureFunctionsHttpTriggerRequest(HttpRequest HttpRequest) : IHttpTriggerRequest
    {
        public HttpRequest HttpRequest { get; } = HttpRequest;
    }
}
