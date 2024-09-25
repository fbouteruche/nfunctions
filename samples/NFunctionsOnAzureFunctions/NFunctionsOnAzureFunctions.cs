using NFunctions;
using NFunctions.Http;

namespace NFunctionsOnAzureFunctions
{
    public class NFunctionsOnAzureFunctions
    {
        [NFunctions("Get")]
        [HttpTrigger(NFunctions.Http.HttpMethod.GET, "api")]
        public HttpTriggerReponse MyFirstFunction(IHttpTriggerRequest request)
        {
            return new HttpTriggerReponse(System.Net.HttpStatusCode.OK, "Hello from NFunction");
        }
    }
}
