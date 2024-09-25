using NFunctions;
using NFunctions.Http;

namespace NFunctionsOnAWSLambda
{
    public class NFunctionsOnAWSLambda
    {
        [NFunctions("Get")]
        [HttpTrigger(NFunctions.Http.HttpMethod.GET, "api")]
        public HttpTriggerReponse MyFirstFunction(IHttpTriggerRequest request)
        {
            return new HttpTriggerReponse(System.Net.HttpStatusCode.OK, "Hello Olivier from NFunction");
        }

    }
}
