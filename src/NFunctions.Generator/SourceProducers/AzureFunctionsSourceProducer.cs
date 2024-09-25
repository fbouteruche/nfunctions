using Microsoft.CodeAnalysis.Text;
using NFunctions.Generator.Models;
using NFunctions.Generator.Models.Http;
using System.Text;

namespace NFunctions.Generator.SourceProducers
{
    internal class AzureFunctionsAdditionalFileProducer : ICloudProviderSourceProducer
    {
        IDictionary<string, SourceText> ICloudProviderSourceProducer.GetSource(NFunctionsModel functionModel)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("using Microsoft.AspNetCore.Http;");
            codeBuilder.AppendLine("using Microsoft.AspNetCore.Mvc;");
            codeBuilder.AppendLine("using Microsoft.Azure.WebJobs;");
            codeBuilder.AppendLine("using Microsoft.Azure.WebJobs.Extensions.Http;");
            codeBuilder.AppendLine("using Microsoft.Extensions.Logging;");
            codeBuilder.AppendLine("using NFunctions.Azure;");
            codeBuilder.AppendLine($"namespace {functionModel.OriginNamespaceName}");
            codeBuilder.AppendLine("{");
            codeBuilder.AppendLine($"\tpublic static class AzureFunctions{functionModel.OriginClassName}");
            codeBuilder.AppendLine("\t{");
            WritePrivateFields(functionModel, codeBuilder);
            WriteFunctionHandler(functionModel, codeBuilder);
            codeBuilder.AppendLine("\t}");
            codeBuilder.AppendLine("}");


            return new Dictionary<string, SourceText>() { { $"AzureFunctions{functionModel.OriginClassName}.g.cs", SourceText.From(codeBuilder.ToString(), Encoding.UTF8) } };
        }

        private void WritePrivateFields(NFunctionsModel functionModel, StringBuilder codeBuilder)
        {
            codeBuilder.AppendLine($"\t\tprivate static {functionModel.OriginClassName} _instance = new {functionModel.OriginClassName}();");
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
            codeBuilder.AppendLine($"\t\t[FunctionName(\"{functionModel.FunctionName}\")]");
            codeBuilder.AppendLine("\t\tpublic static async Task<IActionResult> Run(");
            codeBuilder.AppendLine($"\t\t\t[HttpTrigger(AuthorizationLevel.Anonymous, \"{method}\", Route = \"{route}\")] HttpRequest req,");
            codeBuilder.AppendLine("\t\t\tILogger log)");
            codeBuilder.AppendLine("\t\t{");
            codeBuilder.AppendLine($"\t\t\tvar response = _instance.{functionModel.HandlerMethodName}(new AzureFunctionsHttpTriggerRequest(req));");
            codeBuilder.AppendLine($"\t\t\tstring body = response.Body;");
            codeBuilder.AppendLine("\t\t\treturn new OkObjectResult(body);");
            codeBuilder.AppendLine("\t\t}");
        }
    }
}
