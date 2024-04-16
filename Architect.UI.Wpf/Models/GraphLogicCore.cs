using GraphX.Controls.Models;
using GraphX.Controls;
using GraphX.Logic.Models;

using QuickGraph;

namespace Architect.UI.Wpf.Models
{
    public class GraphLogicCore : GXLogicCore<DependencyVertex, DependencyEdge, BidirectionalGraph<DependencyVertex, DependencyEdge>>
    {
    }
}
