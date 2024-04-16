using System.IO;

using Architect.Contracts;

using GraphX.Common.Models;

namespace Architect.UI.Wpf.Models
{
    public class DependencyVertex : VertexBase
    {
        public DependencyVertex(long id, IDependency dependency)
        {
            ID = id;
            Name = dependency.Name;
            Key = dependency.Key;
            Location = Path.GetFileName(dependency.Location);
        }

        public string Name { get; }

        public string Key { get; }

        public string Location { get; }

        public override string ToString() => Name;
    }
}
