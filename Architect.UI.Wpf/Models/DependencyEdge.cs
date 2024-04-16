using GraphX.Common.Models;

namespace Architect.UI.Wpf.Models
{
    public class DependencyEdge : EdgeBase<DependencyVertex>
    {
        public DependencyEdge(DependencyVertex source, DependencyVertex target, double weight = 1) : base(source, target, weight)
        {
        }        

        public override string ToString() => $"{Source} -> {Target}";
    }
}
