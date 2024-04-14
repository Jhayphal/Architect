using System.Reflection;

namespace ArchitectFramework
{
    public sealed class AssemblyDependency : Dependency<Assembly>
    {
        public AssemblyDependency(Assembly assembly)
        {
            Value = assembly;
            Key = assembly.FullName;
            Name = assembly.GetName().Name;
            Location = assembly.Location;
        }

        public override Assembly Value { get; }

        public override string Key { get; }

        public override string Name { get; }

        public override string Location { get; }
    }
}
