using System;

namespace NFunctions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class NFunctionsAttribute : Attribute
    {
        public string Name { get; }

        public NFunctionsAttribute(string name)
        {
            Name = name;
        }
    }
}
