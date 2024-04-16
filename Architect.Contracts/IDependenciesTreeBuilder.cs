using System.Collections.Generic;

namespace Architect.Contracts
{
    public interface IDependenciesTreeBuilder
    {
        IReadOnlyCollection<IDependencyNode> Build();
    }
}