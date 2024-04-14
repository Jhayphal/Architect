using System;
using System.Collections.Generic;

namespace ArchitectFramework
{
    public interface IDependencyNode : IEquatable<IDependencyNode>
    {
        IDependency Dependency { get; }

        IReadOnlyCollection<IDependencyNode> Ancestors { get; }

        IReadOnlyCollection<IDependencyNode> Children { get; }
    }
}
