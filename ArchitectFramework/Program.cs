using System;
using System.IO;
using System.Linq;

using Architect.Assemblies.Framework;
using Architect.Contracts;

namespace ArchitectFramework
{
    internal class Program
    {
        private const string targetFolder = @"C:\Users\olegn\source\repos\DependencyTargetSample\DependencyTargetSample\bin\Debug";

        static void Main()
        {
            var files = Directory.GetFiles(targetFolder, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(targetFolder, "*.exe", SearchOption.AllDirectories));

            var treeBuilder = new AssemblyDependenciesTreeBuilder();
            treeBuilder.AddAssembliesPaths(files);

            var roots = treeBuilder.Build();

            Console.WriteLine();

            foreach (var node in roots)
            {
                PrintChildrenDependencies(node);

                Console.WriteLine();
            }

            Console.ReadLine();
        }

        private static void PrintChildrenDependencies(IDependencyNode node, int level = 0)
        {
            var shift = new string('\t', level);
            Console.WriteLine(shift + node.Dependency.Name);

            foreach (var child in node.Children)
            {
                PrintChildrenDependencies(child, level + 1);
            }
        }
    }
}
