using NFunctions.Generator.Models;

namespace NFunctions.Generator.AdditionalFileProducers
{
    internal class AzureFunctionsAdditionalFileProducer : IAdditionalFileProducer
    {
        public Task ProduceOrUpdateAsync(NFunctionsModel nFunctionModel, string baseDir, string targetFramework)
        {
            return Task.CompletedTask;
        }
    }
}
