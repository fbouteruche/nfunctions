using System;
using System.Collections.Generic;
using System.Text;

namespace NFunctions.Generator.Models
{
    internal record NFunctionsModel
    {
        internal string OriginAssemblyName { get; }
        internal string OriginClassName { get; }
        internal string OriginNamespaceName { get; }
        internal string HandlerMethodName { get; set; }
        internal string? FunctionName { get; set; }
        internal TriggerType TriggerType { get; set; }
        internal INFunctionsTriggerModel? Trigger { get; set; }
        internal string TargetNode { get; }
        internal NFunctionsModel(string originAssemblyName, string originClassName, string originNamespaceName, string handlerMethodName, string targetNode)
        {
            OriginAssemblyName = originAssemblyName;
            OriginClassName = originClassName;
            OriginNamespaceName = originNamespaceName;
            HandlerMethodName = handlerMethodName;
            TargetNode = targetNode;
        }
    }

    internal enum TriggerType
    {
        NONE,
        HTTP
    }
}
