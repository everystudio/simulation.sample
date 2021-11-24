using System.Collections.Generic;
using System.Linq;

public abstract class Pathfinding
{
    public abstract List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode) where T : IGraphNode;

    protected List<T> GetNeigbours<T>(Dictionary<T, Dictionary<T, float>> edges, T node) where T : IGraphNode
    {
        if (!edges.ContainsKey(node))
        {
            return new List<T>();
        }
        return edges[node].Keys.ToList();
    }
}
