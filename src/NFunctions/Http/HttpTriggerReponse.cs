using System.Net;

namespace NFunctions.Http
{
    public class HttpTriggerReponse : IHttpTriggerResponse
    {
        public string Body { get; }

        public HttpStatusCode StatusCode { get; }

        public HttpTriggerReponse(HttpStatusCode statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }

    }
}
