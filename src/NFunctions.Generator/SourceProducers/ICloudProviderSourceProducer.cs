using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NFunctions.Generator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NFunctions.Generator.SourceProducers
{
    internal interface ICloudProviderSourceProducer
    {
        IDictionary<string, SourceText> GetSource(NFunctionsModel functionModel);
    }
}
