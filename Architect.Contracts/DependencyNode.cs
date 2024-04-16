using System.Collections.Generic;

namespace Architect.Contracts
{
    public abstract class DependencyNode<T> : IDependencyNode
    {
        public abstract Dependency<T> Value { get; }

        public abstract IDependency Dependency { get; }

        public abstract IReadOnlyCollection<IDependencyNode> Ancestors { get; }
        
        public abstract IReadOnlyCollection<IDependencyNode> Children { get; }

        public bool Equals(IDependencyNode other)
            => !(Dependency is null || other is null) && Dependency.Equals(other);

        public override bool Equals(object obj)
            => Equals(obj as IDependencyNode);

        public override int GetHashCode()
            => (Dependency?.GetHashCode()).GetValueOrDefault();

        public override string ToString()
            => Dependency?.ToString();

        public static bool operator ==(DependencyNode<T> a, DependencyNode<T> b)
            => !(a is null || b is null) && a.Equals(b);

        public static bool operator !=(DependencyNode<T> a, DependencyNode<T> b)
            => a is null || b is null || !a.Equals(b);
    }
}
