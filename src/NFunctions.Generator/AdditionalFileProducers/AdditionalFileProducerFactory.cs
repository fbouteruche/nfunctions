namespace NFunctions.Generator.AdditionalFileProducers
{
    internal class AdditionalFileProducerFactory
    {
        private static readonly Dictionary<string, IAdditionalFileProducer> _functionsProviderWriterCollection = new Dictionary<string, IAdditionalFileProducer>
        {
            { "azure", new AzureFunctionsAdditionalFileProducer() },
            { "aws", new AWSLambdaAdditionalFileProducer() }
        };

        internal static IAdditionalFileProducer? Get(string provider)
        {
            return _functionsProviderWriterCollection[provider];
        }
    }
}
