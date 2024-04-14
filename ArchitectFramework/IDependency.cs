using System;

namespace ArchitectFramework
{
    public interface IDependency : IEquatable<IDependency>
    {
        string Key { get; }

        string Name { get; }

        string Location { get; }
    }
}
