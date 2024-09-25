using NFunctions.Generator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NFunctions.Generator.Models.Http
{
    internal class NFunctionHttpTriggerModel : INFunctionsTriggerModel
    {
        public string Method { get; }

        public string Route { get; }

        public NFunctionHttpTriggerModel(string method, string route)
        {
            Method = method;
            Route = route;
        }
    }
}
