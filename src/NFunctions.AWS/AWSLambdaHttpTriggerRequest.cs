using Amazon.Lambda.APIGatewayEvents;
using NFunctions.Http;

namespace NFunctions.AWS
{
    public class AWSLambdaHttpTriggerRequest(APIGatewayProxyRequest Request) : IHttpTriggerRequest
    {
        public APIGatewayProxyRequest Request { get; } = Request;
    }
}
