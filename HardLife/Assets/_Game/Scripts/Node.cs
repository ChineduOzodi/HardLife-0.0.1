using UnityEngine;
public class Node
{
    public float walkSpeed;
    public Vector3 worldPosition;
    public Coord coord;
    public Node parent;

    public int gCost; //cost from node position to the start node
    public int hCost; //cost from the node position to the target node

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(float _walkSpeed, Vector3 _worldPosition)
    {
        walkSpeed = _walkSpeed;
        worldPosition = _worldPosition;
    }
    public static Node NodeFromPosition(Coord pos, LocalMapModel model)
    {
        float walkSpeed = 1;
        Vector3 worldPosition = model.baseMap[pos.x, pos.y].worldPostition;

        //Get correct walk speed modification
        walkSpeed *= model.baseMap[pos.x, pos.y].walkSpeedMod;
        if (model.objectMap[pos.x, pos.y] != null)
        {
            walkSpeed *= model.objectMap[pos.x, pos.y].walkSpeedMod;
        }
        if (model.roadMap != null)
        {
            if (model.roadMap[pos.x, pos.y] != null)
            {
                walkSpeed *= model.roadMap[pos.x, pos.y].walkSpeedMod;
            }
        }
        
        Node node = new Node(walkSpeed, worldPosition);

        node.coord = pos; //set position
        return node;
    }
}