using NFunctions;
using NFunctions.Http;
using System.Net;

namespace DebugNFunctionsToAzureFunctions
{
    
    public class SampleFunction
    {
        [NFunctions("MyFirstFunction")]
        [HttpTrigger(NFunctions.Http.HttpMethod.GET, "api")]
        public HttpTriggerReponse MyFirstFunction(IHttpTriggerRequest request)
        {
            return new HttpTriggerReponse(HttpStatusCode.OK, "Hello from NFunctions");
        }
    }
}
