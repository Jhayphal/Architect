using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArchitectFramework
{
    public sealed class DependenciesTreeBuilder
    {
        private readonly HashSet<IDependencyNode> dependencies = new HashSet<IDependencyNode>();

        public bool Add(IDependencyNode dependency) => dependencies.Add(dependency);

        public IEnumerable<IDependencyNode> GetRoots() => Enumerable.Empty<IDependencyNode>();
    }

    internal class Program
    {
        private const string assemblyAlreadyLoadedExceptionMessage = "Assembly with same name is already loaded";
        private const string targetFolder = @"D:\DataGridExtensions-master\src";

        static void Main()
        {
            var files = Directory.GetFiles(targetFolder, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(targetFolder, "*.exe", SearchOption.AllDirectories))
                .OrderBy(x => x)
                .ToArray();

            var roots = GetRoots(files)
                .Select(r => new AssemblyDependencyNode(r))
                .ToArray();

            var cache = new HashSet<Dependency<Assembly>>();
            foreach (var dependency in roots)
            {
                var localPath = Path.GetDirectoryName(dependency.Value.Location);
                
                var localFiles = files
                    .Where(f => f.StartsWith(localPath))
                    .ToArray();

                LoadChildrenDependencies(dependency, cache, localFiles);
            }

            Console.WriteLine("Push Enter key...");
            Console.ReadLine();
            Console.Clear();

            foreach (var node in roots)
            {
                PrintChildrenDependencies(node);

                Console.WriteLine();
            }
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

        private static bool IsExternalAssembly(Assembly assembly)
            => assembly.GlobalAssemblyCache || !assembly.Location.StartsWith(targetFolder);

        private static HashSet<Dependency<Assembly>> GetRoots(IEnumerable<string> files)
        {
            var result = new HashSet<Dependency<Assembly>>();

            foreach (var item in files)
            {
                Console.WriteLine(item);

                try
                {
                    var assembly = Assembly.LoadFrom(item);

                    Console.WriteLine("Assembly was loaded: " + assembly.FullName);

                    var childDependency = new AssemblyDependency(assembly);
                    if (!result.Add(childDependency))
                    {
                        Console.WriteLine(assembly.FullName + " dependency already exists.");
                    }

                    Console.WriteLine();
                }
                catch (FileLoadException ex) when (assemblyAlreadyLoadedExceptionMessage.Equals(ex))
                {
                    Console.WriteLine(assemblyAlreadyLoadedExceptionMessage);
                }
            }

            return result;
        }

        private static void LoadChildrenDependencies(
            AssemblyDependencyNode ancestorNode,
            HashSet<Dependency<Assembly>> cache,
            IEnumerable<string> assemblies)
        {
            var dependency = ancestorNode.Value;
            if (!cache.Add(dependency))
            {
                return;
            }

            var assembly = dependency.Value;
            foreach (var child in assembly.GetReferencedAssemblies())
            {
                ReadAssembly(ancestorNode, child, cache, assemblies);
            }
        }

        private static void ReadAssembly(
            AssemblyDependencyNode ancestorNode,
            AssemblyName assemblyName,
            HashSet<Dependency<Assembly>> cache,
            IEnumerable<string> assemblies)
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (FileLoadException ex) when (assemblyAlreadyLoadedExceptionMessage.Equals(ex))
            {
                Console.WriteLine(assemblyAlreadyLoadedExceptionMessage);

                return;
            }
            catch
            {
                var predictedAssemblyFileName = $"{assemblyName.Name}.dll";
                var filePath = assemblies.FirstOrDefault(x => Path.GetFileName(x).Equals(predictedAssemblyFileName));
                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                try
                {
                    assembly = Assembly.LoadFrom(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    return;
                }
            }

            Console.WriteLine("Assembly was loaded: " + assembly.FullName);

            if (IsExternalAssembly(assembly))
            {
                Console.WriteLine("Ignored as external dependency.");
            }
            else
            {
                var childDependencyNode = new AssemblyDependencyNode(new AssemblyDependency(assembly));
                ancestorNode.AddChild(childDependencyNode);
                LoadChildrenDependencies(childDependencyNode, cache, assemblies);
            }
        }

        /*
                static void MainOld()
                {
                    const string assemblyAlreadyLoadedExceptionMessage = "Assembly with same name is already loaded";
                    const string targetFolder = @"D:\DataGridExtensions-master\src";

                    var assemblyDependenciesCache = new Dictionary<Assembly, AssemblyName[]>();

                    var files = Directory.GetFiles(targetFolder, "*.dll", SearchOption.AllDirectories)
                        .Union(Directory.GetFiles(targetFolder, "*.exe", SearchOption.AllDirectories))
                        .OrderBy(x => x);

                    foreach (var item in files)
                    {
                        Console.WriteLine(item);

                        try
                        {
                            var assembly = Assembly.LoadFrom(item);
                            Console.WriteLine(assembly.FullName);
                            assemblyDependenciesCache.Add(assembly, assembly.GetReferencedAssemblies());
                        }
                        catch (FileLoadException ex) when (assemblyAlreadyLoadedExceptionMessage.Equals(ex))
                        {
                            Console.WriteLine(ex);
                        }

                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                    }

                    var dependencyGraph = new Dictionary<Assembly, IReadOnlyCollection<Assembly>>();

                    foreach (var assemblyDependencies in assemblyDependenciesCache)
                    {
                        var dependencies = new List<Assembly>();
                        dependencyGraph.Add(assemblyDependencies.Key, dependencies);

                        foreach (var assemblyName in assemblyDependencies.Value)
                        {
                            var referencedAssembly = assemblyDependenciesCache.Keys.FirstOrDefault(a => a.FullName == assemblyName.FullName);
                            if (referencedAssembly is null)
                            {
                                try
                                {
                                    referencedAssembly = Assembly.Load(assemblyName);

                                    dependencies.Add(referencedAssembly);
                                    dependencyGraph.Add(referencedAssembly, new List<Assembly>());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }

                            if (!(referencedAssembly is null))
                            {
                                dependencies.Add(referencedAssembly);
                            }
                        }
                    }
                }
        */
    }
}
