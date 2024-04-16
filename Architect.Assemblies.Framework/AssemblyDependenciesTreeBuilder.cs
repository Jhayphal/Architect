using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Architect.Contracts;

namespace Architect.Assemblies.Framework
{
    public sealed class AssemblyDependenciesTreeBuilder : IDependenciesTreeBuilder
    {
        private readonly List<string> assemblies = new List<string>();

        public void AddAssemblyPath(string fileName) => assemblies.Add(fileName);

        public void AddAssembliesPaths(IEnumerable<string> filesNames) => assemblies.AddRange(filesNames);

        public IReadOnlyCollection<IDependencyNode> Build()
        {
            if (assemblies.Count == 0)
            {
                return Array.Empty<IDependencyNode>();
            }

            var namesCache = assemblies.ToDictionary(p => Path.GetFileNameWithoutExtension(p));
            var loadedAssemblies = new Dictionary<string, AssemblyDependencyNode>();

            foreach (var path in assemblies)
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.LoadFrom(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cannot load assembly '{path}'. Error: {ex.Message}");
                    continue;
                }

                Console.WriteLine("Assembly was loaded: " + assembly.FullName);

                if (loadedAssemblies.ContainsKey(assembly.FullName))
                {
                    Console.WriteLine("Assembly already checked. Skipped.");
                    continue;
                }

                var dependency = new AssemblyDependencyNode(new AssemblyDependency(assembly));
                loadedAssemblies.Add(assembly.FullName, dependency);
            }

            foreach (var dependencyNode in loadedAssemblies.Values)
            {
                var dependency = dependencyNode.Value;
                Console.WriteLine($"Building assembly dependencies for '{dependency.Name}'...");

                foreach (var reference in dependency.Value.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.TryGetValue(reference.FullName, out var referencedDependency))
                    {
                        Console.WriteLine($"Referenced dependency '{reference.FullName}' was not loaded. Skipped.");
                        continue;
                    }

                    if (dependencyNode.AddChild(referencedDependency))
                    {
                        referencedDependency.AddAncestor(dependencyNode);
                    }
                }
            }

            return loadedAssemblies.Values
                .Where(d => d.Ancestors.Count == 0)
                .ToArray();
        }
    }
}
