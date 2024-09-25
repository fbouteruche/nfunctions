namespace NFunctions.Generator.SourceProducers
{
    internal class CloudProviderAdditionalFileProducerFactory
    {
        private static readonly Dictionary<string, ICloudProviderSourceProducer> _functionsProviderWriterCollection = new Dictionary<string, ICloudProviderSourceProducer>
        {
            { "azure", new AzureFunctionsAdditionalFileProducer() },
            { "aws", new AWSLambdaAdditionalFileProducer() }
        };

        internal static ICloudProviderSourceProducer? Get(string provider)
        {
            return _functionsProviderWriterCollection[provider];
        }
    }
}
