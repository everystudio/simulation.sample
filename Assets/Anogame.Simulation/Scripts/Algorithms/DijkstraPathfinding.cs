using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace anogame_strategy
{
    public class DijkstraPathfinding : Pathfinding
    {
        public Dictionary<TileInfo, List<TileInfo>> findAllPaths(Dictionary<TileInfo, Dictionary<TileInfo, float>> edges, TileInfo originNode)
        {
            IPriorityQueue<TileInfo> frontier = new HeapPriorityQueue<TileInfo>();
            frontier.Enqueue(originNode, 0);

            Dictionary<TileInfo, TileInfo> cameFrom = new Dictionary<TileInfo, TileInfo>();
            cameFrom.Add(originNode, default(TileInfo));
            Dictionary<TileInfo, float> costSoFar = new Dictionary<TileInfo, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                //Debug.Log(neighbours.Count);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    //Debug.Log(newCost);
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
            }

            Dictionary<TileInfo, List<TileInfo>> paths = new Dictionary<TileInfo, List<TileInfo>>();
            foreach (TileInfo destination in cameFrom.Keys)
            {
                List<TileInfo> path = new List<TileInfo>();
                var current = destination;
                while (!current.Equals(originNode))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                paths.Add(destination, path);
            }
            return paths;
        }
        public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode)
        {
            IPriorityQueue<T> frontier = new HeapPriorityQueue<T>();
            frontier.Enqueue(originNode, 0);

            Dictionary<T, T> cameFrom = new Dictionary<T, T>();
            cameFrom.Add(originNode, default(T));
            Dictionary<T, float> costSoFar = new Dictionary<T, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
                if (current.Equals(destinationNode)) break;
            }
            List<T> path = new List<T>();
            if (!cameFrom.ContainsKey(destinationNode))
                return path;

            path.Add(destinationNode);
            var temp = destinationNode;

            while (!cameFrom[temp].Equals(originNode))
            {
                var currentPathElement = cameFrom[temp];
                path.Add(currentPathElement);

                temp = currentPathElement;
            }

            return path;
        }
    }
}