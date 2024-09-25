using System;

namespace NFunctions.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpTriggerAttribute(HttpMethod method, string route) : Attribute
    {
        public HttpMethod Method { get; } = method;

        public string Route { get; } = route;
    }
}
