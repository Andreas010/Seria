using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seria;

public interface ISeriaObject
{
    public void Serialize(SeriaBuffer buffer);
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class ObjectNameAttribute : Attribute
{
    public string Name { get; }

    public ObjectNameAttribute(string name)
    {
        Name = name;
    }
}
