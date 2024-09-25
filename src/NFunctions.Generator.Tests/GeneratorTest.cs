using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace NFunctions.Generator.Tests
{
    public class GeneratorTest
    {
        [Fact]
        public void DebugTest()
        {
            var compilation = CSharpCompilation.Create("TestProject",
                new[] { CSharpSyntaxTree.ParseText("using NFunction;\r\n\r\nnamespace SampleNFunction\r\n{\r\n    \r\n    public class SampleFunction\r\n    {\r\n        [NFunction]\r\n        [HttpTrigger]\r\n        public void MyFirstFunction()\r\n        {\r\n\r\n        }\r\n    }\r\n}") },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new NFunctionsGenerator();
            var sourceGenerator = generator.AsSourceGenerator();

            // trackIncrementalGeneratorSteps allows to report info about each step of the generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: [sourceGenerator],
                driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

            // Run the generator
            driver = driver.RunGenerators(compilation);
        }
    }
}