using NFunctions.Generator.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace NFunctions.Generator.AdditionalFileProducers
{
    internal class AWSLambdaAdditionalFileProducer : IAdditionalFileProducer
    {
        public async Task ProduceOrUpdateAsync(NFunctionsModel nFunctionModel, string baseDir, string targetFramework)
        {
            await EnsureServerlessTemplateFileExistence(baseDir);

            await UpdateAsync(nFunctionModel, baseDir, targetFramework);
        }

        private async Task UpdateAsync(NFunctionsModel nFunctionModel, string baseDir, string targetFramework)
        {
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
            JsonObject? cloudFormationTemplate = null;
            using (var fileReader = File.OpenRead(Path.Combine(baseDir, "serverless.template")))
            {
                cloudFormationTemplate = await JsonSerializer.DeserializeAsync<JsonObject>(fileReader);
            }
#pragma warning restore RS1035 // Do not use APIs banned for analyzers

            if (cloudFormationTemplate is not null)
            {
                JsonNode? resourcesNode = cloudFormationTemplate["Resources"];
                if (resourcesNode is not null && nFunctionModel.FunctionName is not null)
                {
                    resourcesNode[nFunctionModel.FunctionName] = JsonSerializer.SerializeToNode(new
                    {
                        Type = "AWS::Serverless::Function",
                        Properties = new
                        {
                            Architectures = new[] { "x86_64" },
                            Handler = GetFullyQualifyName(nFunctionModel),
                            Runtime = ConvertTargetFrameworkToRuntime(targetFramework),
                            CodeUri = "",
                            MemorySize = 256,
                            Timeout = 30,
                            Policies = new[] { "AWSLambdaBasicExecutionRole" },
                            Events = new
                            {
                                RootEvent = new
                                {
                                    Type = "Api",
                                    Properties = new
                                    {
                                        Path = "/",
                                        Method = "GET"
                                    }
                                }
                            }
                        }
                    });
                }

#pragma warning disable RS1035 // Do not use APIs banned for analyzers
                using (var fileWriter = File.OpenWrite(Path.Combine(baseDir, "serverless.template")))
                {
                    await JsonSerializer.SerializeAsync(fileWriter, cloudFormationTemplate, new JsonSerializerOptions() { WriteIndented = true });
                }
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
            }
        }

        private string ConvertTargetFrameworkToRuntime(string targetFramework)
        {
            switch (targetFramework)
            {
                case "net8.0":
                    return "dotnet8";
                case "net6.0":
                    return "dotnet6";
                default:
                    return "";
            }
        }

        private string GetFullyQualifyName(NFunctionsModel nFunctionModel)
        {
            StringBuilder fqnBuilder = new();
            fqnBuilder.Append(nFunctionModel.OriginAssemblyName);
            fqnBuilder.Append("::");
            fqnBuilder.Append(nFunctionModel.OriginNamespaceName);
            fqnBuilder.Append(".");
            fqnBuilder.Append($"AWSLambda{nFunctionModel.OriginClassName}");
            fqnBuilder.Append("::");
            fqnBuilder.Append(nFunctionModel.FunctionName);
            return fqnBuilder.ToString();
        }

        private async Task EnsureServerlessTemplateFileExistence(string baseDir)
        {
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
            if (!File.Exists(Path.Combine(baseDir, "serverless.template")))
            {
                using (var streamWriter = File.CreateText(Path.Combine(baseDir, "serverless.template")))
                {
                    await streamWriter.WriteAsync("{\r\n  \"AWSTemplateFormatVersion\": \"2010-09-09\",\r\n  \"Transform\": \"AWS::Serverless-2016-10-31\",\r\n  \"Description\": \"An AWS Serverless Application.\",\r\n  \"Resources\": {\r\n  },\r\n  \"Outputs\": {\r\n    \"ApiURL\": {\r\n      \"Description\": \"API endpoint URL for Prod environment\",\r\n      \"Value\": {\r\n        \"Fn::Sub\": \"https://${ServerlessRestApi}.execute-api.${AWS::Region}.amazonaws.com/Prod/\"\r\n      }\r\n    }\r\n  }\r\n}");
                }
            }
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
        }
    }
}
