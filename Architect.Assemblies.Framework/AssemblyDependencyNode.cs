using System.Collections.Generic;
using System.Reflection;

using Architect.Contracts;

namespace Architect.Assemblies.Framework
{
    public sealed class AssemblyDependencyNode : DependencyNode<Assembly>
    {
        private readonly HashSet<DependencyNode<Assembly>> ancestors = new HashSet<DependencyNode<Assembly>>();
        private readonly HashSet<DependencyNode<Assembly>> children = new HashSet<DependencyNode<Assembly>>();

        public AssemblyDependencyNode(Dependency<Assembly> dependency)
        {
            Value = dependency;
            Dependency = dependency;
        }

        public override Dependency<Assembly> Value { get; }

        public override IDependency Dependency { get; }

        public override IReadOnlyCollection<IDependencyNode> Ancestors => ancestors;

        public override IReadOnlyCollection<IDependencyNode> Children => children;

        public bool AddAncestor(DependencyNode<Assembly> ancestor) => ancestors.Add(ancestor);

        public bool AddChild(DependencyNode<Assembly> child) => children.Add(child);
    }
}
