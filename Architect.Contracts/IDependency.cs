using System;

namespace Architect.Contracts
{
    public interface IDependency : IEquatable<IDependency>
    {
        string Key { get; }

        string Name { get; }

        string Location { get; }
    }
}
