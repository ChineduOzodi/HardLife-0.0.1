using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;

    LocalMapModel localMap;
    Node[,] grid;
    List<Node> path;

    // Use this for initialization
    void Awake()
    {
        localMap = GameObject.FindGameObjectWithTag("LocalGen").GetComponent<LocalMapController>().model;
        grid = new Node[localMap.localSizeX, localMap.localSizeY];

    }

    void Update()
    {
        FindLocalPath(Coord.Vector3ToCoord(seeker.position - localMap.worldBottomLeft), Coord.Vector3ToCoord(target.position - localMap.worldBottomLeft));
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(10, 10));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                if (n != null)
                {
                    Gizmos.color = (n.walkSpeed != 0) ? Color.white : Color.red;
                    if (path != null)
                    {
                        if (path.Contains(n))
                            Gizmos.color = Color.black;
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one);
                }
                

            }
        }
    }
    public void FindLocalPath(Coord startPos, Coord targetPos)
    {
        Node startNode = Node.NodeFromPosition(startPos, localMap);
        Node targetNode = Node.NodeFromPosition(targetPos, localMap);

        startNode.coord = startPos;
        targetNode.coord = targetPos;

        //Save Nodes to Grid
        grid[startPos.x, startPos.y] = startNode;
        grid[targetPos.x, targetPos.y] = targetNode;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(grid[startPos.x, startPos.y], grid[targetPos.x, targetPos.y]);
                return;
            }

            foreach (Node neighbor in GetNeighbors(currentNode)){

                if (neighbor.walkSpeed == 0 || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) * (int)(currentNode.walkSpeed * 100);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }

            }

        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
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
