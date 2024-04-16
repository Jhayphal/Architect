using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

using Architect.Assemblies.Framework;
using Architect.UI.Wpf.Models;

using GraphX.Common.Enums;

namespace Architect.UI.Wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string targetFolder = @"C:\Users\olegn\source\repos\DependencyTargetSample\DependencyTargetSample\bin\Debug";

        public MainWindow()
        {
            InitializeComponent();

            var files = Directory.GetFiles(targetFolder, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(targetFolder, "*.exe", SearchOption.AllDirectories));

            var treeBuilder = new AssemblyDependenciesTreeBuilder();
            treeBuilder.AddAssembliesPaths(files);

            var roots = treeBuilder.Build();
            var allNodes = new Dictionary<Contracts.IDependency, DependencyVertex>();
            CollectAllNodes(roots, allNodes);
            var graph = new DependencyGraph();
            BuildGraph(graph, roots, allNodes);

            var logic = new GraphLogicCore();
            area.LogicCore = logic;
            logic.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Tree;
            logic.DefaultLayoutAlgorithmParams.Seed = 1;
            logic.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            logic.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 200;
            logic.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 100;

            logic.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.None;
            logic.EdgeCurvingEnabled = false;

            area.GenerateGraph(graph);
            area.ShowAllEdgesLabels(false);
        }

        private void CollectAllNodes(
            IEnumerable<Contracts.IDependencyNode> nodes,
            Dictionary<Contracts.IDependency, DependencyVertex> result)
        {
            foreach (var node in nodes)
            {
                if (!result.ContainsKey(node.Dependency))
                {
                    result.Add(node.Dependency, new DependencyVertex(result.Count, node.Dependency));
                }

                if (node.Children.Count > 0)
                {
                    CollectAllNodes(node.Children, result);
                }
            }
        }

        private void BuildGraph(
            DependencyGraph graph,
            IEnumerable<Contracts.IDependencyNode> nodes,
            IReadOnlyDictionary<Contracts.IDependency, DependencyVertex> vertexes)
        {
            foreach (var parentNode in nodes)
            {
                var parentVertext = vertexes[parentNode.Dependency];
                graph.AddVertex(parentVertext);

                if (parentNode.Children.Count > 0)
                {
                    foreach (var childNode in parentNode.Children)
                    {
                        var childVertex = vertexes[childNode.Dependency];
                        graph.AddVertex(childVertex);
                        graph.AddEdge(new DependencyEdge(parentVertext, childVertex));
                    }

                    BuildGraph(graph, parentNode.Children, vertexes);
                }
            }
        }
    }
}
