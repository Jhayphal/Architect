namespace ArchitectFramework
{
    public abstract class Dependency<T> : IDependency
    {
        public abstract T Value { get; }

        public abstract string Key { get; }

        public abstract string Name { get; }
        
        public abstract string Location { get; }

        public bool Equals(IDependency other)
            => string.Equals(Key, other?.Key);

        public override bool Equals(object obj)
            => Equals(obj as IDependency);

        public override int GetHashCode()
            => Key.GetHashCode();

        public override string ToString()
            => Key;

        public static bool operator ==(Dependency<T> a, Dependency<T> b)
            => !(a is null || b is null) && a.Equals(b);

        public static bool operator !=(Dependency<T> a, Dependency<T> b)
            => a is null || b is null || !a.Equals(b);
    }
}
