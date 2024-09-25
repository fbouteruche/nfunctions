using NFunctions;
using NFunctions.Http;
using System.Net;

namespace DebugNFunctionsToAWSLambda
{

    public class SampleFunction
    {
        [NFunctions("Get")]
        [HttpTrigger(NFunctions.Http.HttpMethod.GET, "api")]
        public HttpTriggerReponse MyFirstFunction(IHttpTriggerRequest request)
        {
            return new HttpTriggerReponse(HttpStatusCode.OK, "Hello from NFunctions");
        }
    }
}