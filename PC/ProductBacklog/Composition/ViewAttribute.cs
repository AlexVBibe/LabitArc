using System;

namespace Labit.Composition
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class ViewAttribute : Attribute
    {
        public string TypeName { get; private set; }
        public string InstanceName { get; private set; }

        public ViewAttribute(string typeName, string instanceName)
        {
            this.TypeName = typeName;
            this.InstanceName = instanceName;
        }

        public ViewAttribute(string typeName)
        {
            this.TypeName = typeName;
        }
    }
}
