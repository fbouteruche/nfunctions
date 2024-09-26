# NFunctions (alpha)

NFunctions is a C# open source serverless function framework that aims to free your code from AWS Lambda or Azure Functions
boilerplate code while still leveraging those FaaS services.

You just use NFunctions agnostic attributes to tag your function handler methods and define their triggers. 
NFunctions source generator then automatically generates the AWS Lambda or Azure boilerplate code for you based
on the NFunctions cloud provider NuGet package you've added to your project: `NFunctions.AWS` or `NFunctions.Azure`.

A typical NFunctions handler method looks like the below code. The `NFunctions` attribute tags your method
as a function handler method. You can pass the name of the function to attribute. The `HttpTrigger` attribute
defines that the function exectution is triggered by a GET http request.

```csharp

    public class NFunctionsOnAWSLambda
    {
        [NFunctions("Get")]
        [HttpTrigger(NFunctions.Http.HttpMethod.GET, "api")]
        public HttpTriggerReponse MyFirstFunction(IHttpTriggerRequest request)
        {
            return new HttpTriggerReponse(System.Net.HttpStatusCode.OK, "Hello from NFunction");
        }

    }
```

## Samples

In the `samples` folder, you have two sample projects that showcase that the same source code can be deployed
seamlessly on AWS or Azure. You just have to open the `NFunctionsSamples` solution and build the projects.
If you use Visual Studio 2022, you can use the AWS Toolkit extension to deploy on AWS or the built-in Azure 
tooling to deploy on Azure.

## Source code

In the `src` folder, you have the `NFunctions` solution that you can use to explore and build the NFunctions
NuGet packages.

## Project status

This project is in its early stage. It aims at exploring the art of possible to abstract your serverless function 
code away from AWS Lambda and Azure Functions without sacrifiying to Kubernetes.

## How to provide feedback

We are looking for feedback. Please, open an issue to record your feedback in our GitHub repository.