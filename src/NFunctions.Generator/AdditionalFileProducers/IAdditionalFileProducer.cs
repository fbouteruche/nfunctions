using NFunctions.Generator.Models;

namespace NFunctions.Generator.AdditionalFileProducers
{
    internal interface IAdditionalFileProducer
    {
        Task ProduceOrUpdateAsync(NFunctionsModel nFunctionModel, string baseDir, string targetFramework);
    }
}
