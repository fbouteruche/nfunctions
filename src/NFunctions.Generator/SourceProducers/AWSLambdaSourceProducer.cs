using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;
using NFunctions.Generator.Models;
using NFunctions.Generator.Models.Http;

namespace NFunctions.Generator.SourceProducers
{
    internal class AWSLambdaAdditionalFileProducer : ICloudProviderSourceProducer
    {
        IDictionary<string, SourceText> ICloudProviderSourceProducer.GetSource(NFunctionsModel functionModel)
        {
            var results = new Dictionary<string, SourceText>();
            results.Add($"AWSLambda{functionModel.OriginClassName}.g.cs", GenerateCSharpFile(functionModel));
            return results;
        }

        private SourceText GenerateCSharpFile(NFunctionsModel functionModel)
        {

            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine("using Amazon.Lambda.APIGatewayEvents;");
            codeBuilder.AppendLine("using Amazon.Lambda.Core;");
            codeBuilder.AppendLine("using System.Net;");
            codeBuilder.AppendLine(String.Concat("using ", nameof(NFunctions), ".AWS;"));
            codeBuilder.AppendLine("[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]");
            codeBuilder.AppendLine($"namespace {functionModel.OriginNamespaceName}");
            codeBuilder.AppendLine("{");
            codeBuilder.AppendLine($"\tpublic class AWSLambda{functionModel.OriginClassName}");
            codeBuilder.AppendLine("\t{");
            WritePrivateFields(functionModel, codeBuilder);
            WriteConstructor(functionModel, codeBuilder);
            WriteFunctionHandler(functionModel, codeBuilder);
            codeBuilder.AppendLine("\t}");
            codeBuilder.AppendLine("}");

            return SourceText.From(codeBuilder.ToString(), Encoding.UTF8);
        }

        private void WritePrivateFields(NFunctionsModel functionModel, StringBuilder codeBuilder)
        {
            codeBuilder.AppendLine($"\t\tprivate {functionModel.OriginClassName} _instance = new {functionModel.OriginClassName}();");
            codeBuilder.AppendLine();
        }

        private void WriteConstructor(NFunctionsModel functionModel, StringBuilder codeBuilder)
        {
            codeBuilder.AppendLine($"\t\tpublic AWSLambda{functionModel.OriginClassName}(){{}}");
            codeBuilder.AppendLine();
        }

        private void WriteFunctionHandler(NFunctionsModel functionModel, StringBuilder codeBuilder)
        {
            if (functionModel.TriggerType == TriggerType.HTTP)
            {
                WriteHttpTriggerFunctionHandler(functionModel, codeBuilder);
            }
        }

        private void WriteHttpTriggerFunctionHandler(NFunctionsModel functionModel, StringBuilder codeBuilder)
        {
            string? method = functionModel.Trigger is null ? null : ((NFunctionHttpTriggerModel)functionModel.Trigger).Method;
            string? route = functionModel.Trigger is null ? null : ((NFunctionHttpTriggerModel)functionModel.Trigger).Route;
            codeBuilder.AppendLine($"\t\tpublic APIGatewayProxyResponse {functionModel.FunctionName}(APIGatewayProxyRequest request, ILambdaContext context)");
            codeBuilder.AppendLine("\t\t{");
            codeBuilder.AppendLine($"\t\t\tvar response = _instance.{functionModel.HandlerMethodName}(new AWSLambdaHttpTriggerRequest(request));");
            codeBuilder.AppendLine("\t\t\treturn new APIGatewayProxyResponse()");
            codeBuilder.AppendLine("\t\t\t{");
            codeBuilder.AppendLine("\t\t\t\tStatusCode = (int)response.StatusCode,");
            codeBuilder.AppendLine("\t\t\t\tBody = response.Body");
            codeBuilder.AppendLine("\t\t\t};");
            codeBuilder.AppendLine("\t\t}");
        }
    }
}
