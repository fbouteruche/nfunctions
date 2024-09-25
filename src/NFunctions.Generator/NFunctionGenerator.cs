using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NFunctions.Generator.AdditionalFileProducers;
using NFunctions.Generator.Models;
using NFunctions.Generator.Models.Http;
using NFunctions.Generator.SourceProducers;
using NFunctions.Http;
using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace NFunctions.Generator
{
    [Generator]
    public class NFunctionsGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)

        {
            var projDirectory = context.AnalyzerConfigOptionsProvider
               .Select((x, _) => x.GlobalOptions
               .TryGetValue("build_property.MSBuildProjectDirectory", out var projectDirectory) ? projectDirectory : null);

            var targetFramework = context.AnalyzerConfigOptionsProvider
               .Select((x, _) => x.GlobalOptions
               .TryGetValue("build_property.TargetFramework", out var targetFramework) ? targetFramework : null);

            var cloudProvider = context.MetadataReferencesProvider.Where(SelectCloudProviderReference).Collect();

            var functionToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName(String.Concat(nameof(NFunctions), ".", nameof(NFunctionsAttribute)),
                predicate: static (_, _) => true,
                transform: (ctx, _) => GenerateFunctionModel(ctx));

            var combineWithConfig = functionToGenerate.Combine(cloudProvider);

            var combineWithProjDirectory = combineWithConfig.Combine(projDirectory).Select((pair, _) => (pair.Left.Left, pair.Left.Right, pair.Right));

            var combineWithTargetFramework = combineWithProjDirectory.Combine(targetFramework).Select((pair, _) => (pair.Left.Left, pair.Left.Item2, pair.Left.Item3, pair.Right));

            context.RegisterImplementationSourceOutput(combineWithTargetFramework, async (spc, source) => await Execute(spc, source));
        }

        private bool SelectCloudProviderReference(MetadataReference metadataReference)
        {
            return metadataReference.Display is null ? false : metadataReference.Display.Contains("NFunctions.AWS") || metadataReference.Display.Contains("NFunctions.Azure");
        }

        private async Task Execute(SourceProductionContext spc, (NFunctionsModel functionModel, ImmutableArray<MetadataReference> metadataReferences, string? baseDir, string? targetFramework) source)
        {
            var faasProvider = RetrieveFaaSProvider(source.metadataReferences.FirstOrDefault()?.Display);
            if (faasProvider != null)
            {
                ICloudProviderSourceProducer? cloudProviderSourceProducer = CloudProviderAdditionalFileProducerFactory.Get(faasProvider);
                IAdditionalFileProducer? cloudProviderAdditionalFileProducer = AdditionalFileProducerFactory.Get(faasProvider);

                if (cloudProviderSourceProducer is not null && cloudProviderAdditionalFileProducer is not null && source.baseDir is not null)
                {
                    foreach (var file in cloudProviderSourceProducer.GetSource(source.functionModel))
                    {
                        spc.AddSource(file.Key, file.Value);
                        await cloudProviderAdditionalFileProducer.ProduceOrUpdateAsync(source.functionModel, source.baseDir, source.targetFramework);
                    }
                }
            }
        }

        private static string? RetrieveFaaSProvider(string? cloudProviderAssemblyName)
        {
            if (cloudProviderAssemblyName is null) return null;

            if (cloudProviderAssemblyName.Contains("AWS"))
            {
                return "aws";
            }
            else if (cloudProviderAssemblyName.Contains("Azure"))
            {
                return "azure";
            }
            else
            {
                return null;
            }
        }

        private NFunctionsModel GenerateFunctionModel(GeneratorAttributeSyntaxContext ctx)
        {
            string? assemblyName = RetrieveAssemblyName(ctx);
            string? namespaceName = RetrieveNamespace(ctx);
            string? className = RetrieveClassName(ctx);
            string? handlerMethodName = RetrieveHandlerMethodName(ctx);

            if (assemblyName is null)
            {
                throw new Exception("Unable to determine the assembly name of the Nfunctions");
            }
            if (className is null)
            {
                throw new Exception("Unable to determine the class name of the Nfunctions");
            }
            if(namespaceName is null)
            {
                throw new Exception("Unable to determine the namespace of the Nfunctions");
            }
            if (handlerMethodName is null)
            {
                throw new Exception("Unable to determine the handler method name of the Nfunctions");
            }

            var nFunctionModel = new NFunctionsModel(assemblyName, className, namespaceName, handlerMethodName, ctx.TargetNode.ToFullString());

            foreach(var attributeData in ctx.TargetSymbol.GetAttributes())
            {
                ProcessFunctionAttributes(attributeData, nFunctionModel);
            }

            return nFunctionModel;
        }
        private void ProcessFunctionAttributes(AttributeData attributeData, NFunctionsModel nFunctionModel)
        {
            if(attributeData.AttributeClass?.Name == nameof(NFunctionsAttribute))
            {
                ProcessNFunctionAttribute(attributeData, nFunctionModel);
            }
            else if(attributeData.AttributeClass?.Name == nameof(HttpTriggerAttribute))
            {
                ProcessHttpTriggerAttribute(attributeData, nFunctionModel);
            }
        }

        private static void ProcessHttpTriggerAttribute(AttributeData attributeData, NFunctionsModel nFunctionModel)
        {
            nFunctionModel.TriggerType = TriggerType.HTTP;
            string method = attributeData.ConstructorArguments[0].Value switch
            {
                0 => "GET",
                1 => "POST",
                2 => "PUT",
                3 => "DELETE",
                _ => throw new ArgumentOutOfRangeException()
            };
            string route = attributeData.ConstructorArguments[1].Value?.ToString() ?? "/";
            nFunctionModel.Trigger = new NFunctionHttpTriggerModel(method, route);
        }

        private void ProcessNFunctionAttribute(AttributeData attributeData, NFunctionsModel nFunctionModel)
        {
            nFunctionModel.FunctionName = attributeData.ConstructorArguments[0].Value?.ToString();
        }

        private ISymbol? RetrieveParentSymbol(GeneratorAttributeSyntaxContext ctx)
        {
            ISymbol? classContainerSymbol = ctx.TargetNode.Parent is not null ? ctx.SemanticModel.GetDeclaredSymbol(ctx.TargetNode.Parent) : null;
            return classContainerSymbol;
        }

        private string? RetrieveAssemblyName(GeneratorAttributeSyntaxContext ctx)
        {
            return RetrieveParentSymbol(ctx)?.ContainingAssembly?.Name;
        }


        private string? RetrieveNamespace(GeneratorAttributeSyntaxContext ctx)
        {
            return RetrieveParentSymbol(ctx)?.ContainingNamespace?.Name;
        }

        private string? RetrieveClassName(GeneratorAttributeSyntaxContext ctx)
        {
            return RetrieveParentSymbol(ctx)?.Name;
        }

        private string? RetrieveHandlerMethodName(GeneratorAttributeSyntaxContext ctx)
        {
            return ctx.TargetSymbol.Name;
        }
    }
}
