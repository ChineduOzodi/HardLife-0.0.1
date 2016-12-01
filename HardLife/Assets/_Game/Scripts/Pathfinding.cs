using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{

    public bool displayGizmos = false;
    PathRequestManager requestManager;
    
    
    LocalMapModel localMap;
    Node[,] grid;
    List<Node> path;

    public int maxLocalSize
    {
        get
        {
            return localMap.localSizeX * localMap.localSizeY;
        }
    }

    // Use this for initialization
    void Awake()
    {
        localMap = GameObject.FindGameObjectWithTag("LocalGen").GetComponent<LocalMapController>().model;
        grid = new Node[localMap.localSizeX, localMap.localSizeY];
        requestManager = GetComponent<PathRequestManager>();

    }

    internal void StartFindPath(Coord startPos, Coord targetPos)
    {
        StartCoroutine(FindLocalPath(startPos, targetPos));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(10, 10));
        if (grid != null && displayGizmos)
        {
            foreach (Node n in grid)
            {
                if (n != null)
                {
                    Gizmos.color = new Color(n.fCost / 100f, 1, 1, .8f);
                    Gizmos.DrawCube(n.worldPosition, Vector3.one);
                }


            }
        }
    }
    IEnumerator FindLocalPath(Coord startPos, Coord targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node[] waypoints = new Node[0];
        bool pathSuccess = false;

        Node startNode = Node.NodeFromPosition(startPos, localMap);
        Node targetNode = Node.NodeFromPosition(targetPos, localMap);

        startNode.coord = startPos;
        targetNode.coord = targetPos;

        //Save Nodes to Grid
        grid[startPos.x, startPos.y] = startNode;
        grid[targetPos.x, targetPos.y] = targetNode;

        if (startNode.walkSpeed > 0 && targetNode.walkSpeed > 0)
        {
            Heap<Node> openSet = new Heap<Node>(maxLocalSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found:" + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbor in GetNeighbors(currentNode))
                {

                    if (neighbor.walkSpeed == 0 || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) / (currentNode.walkSpeed);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else
                            openSet.UpdateItem(neighbor);
                    }

                }

            }
        }

        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(grid[startPos.x, startPos.y], grid[targetPos.x, targetPos.y]);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Node[] RetracePath(Node startNode, Node endNode)
    {
        path = new List<Node>();
        Node[] waypoints;
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    Node[] SimplifyPath(List<Node> path)
    {
        List<Node> waypoints = new List<Node>();

        Coord directionOld = Coord.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Coord directionNew = path[i - 1].coord - path[i].coord;

            if (directionNew != directionOld)
            {
                waypoints.Add(path[i]);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.coord.x - nodeB.coord.x);
        int distY = Mathf.Abs(nodeA.coord.y - nodeB.coord.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 3; x++)
        {
            for (int y = -1; y <= 3; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.coord.x + x;
                int checkY = node.coord.y + y;

                if (checkX >= 0 && checkX < localMap.localSizeX && checkY >= 0 && checkY < localMap.localSizeY)
                {
                    if (grid[checkX, checkY] == null)
                    {
                        neighbors.Add(Node.NodeFromPosition(new Coord(checkX, checkY), localMap));
                        grid[checkX, checkY] = neighbors[neighbors.Count - 1];
                    }
                    else
                        neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }
}
