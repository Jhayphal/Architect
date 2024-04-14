using System.Reflection;

namespace Architect
{
    internal class Program
    {
        static void Main()
        {
            const string targetFolder = @"D:\DataGridExtensions-master\src";
            var files = Directory.GetFiles(targetFolder, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(targetFolder, "*.exe", SearchOption.AllDirectories))
                .Order();

            foreach (var item in files)
            {
                try
                {
                    Console.WriteLine(item);

                    var assemblyDirectory = Path.GetDirectoryName(item)!;
                    if (!Directory.GetCurrentDirectory().Equals(assemblyDirectory))
                    {
                        Directory.SetCurrentDirectory(assemblyDirectory);
                    }
                    var assembly = Assembly.LoadFrom(item);
                    Console.WriteLine(assembly.FullName);

                    Console.WriteLine("Dependencies:");
                    foreach (var dependency in assembly.GetReferencedAssemblies())
                    {
                        try
                        {
                            assembly = Assembly.Load(dependency);
                            Console.WriteLine($"Loaded dependency: {assembly.FullName}");
                        }
                        catch
                        {
                            Console.WriteLine($"Cannot load dependency: {dependency.FullName}");
                        }
                    }
                }
                catch (FileLoadException ex) when ("Assembly with same name is already loaded".Equals(ex))
                {
                    // Console.WriteLine(ex);
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
