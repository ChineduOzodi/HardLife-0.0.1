using System;
using UnityEngine;
public class Node : IHeapItem<Node>
{
    public float walkSpeed;
    public Vector3 worldPosition;
    public Coord coord;
    public Node parent;

    public float gCost; //cost from node position to the start node
    public float hCost; //cost from the node position to the target node
    int heapIndex;

    public float fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public Node(float _walkSpeed, Vector3 _worldPosition)
    {
        walkSpeed = _walkSpeed;
        worldPosition = _worldPosition;
    }
    public static Node NodeFromPosition(Coord pos, LocalMapModel model)
    {
        float walkSpeed = 1;
        Vector3 worldPosition = model.baseMap[ArrayHelper.ElementIndex(pos.x, pos.y,model.localSizeX)].worldPostition;

        //Get correct walk speed modification
        walkSpeed *= model.baseMap[ArrayHelper.ElementIndex(pos.x, pos.y,model.localSizeX)].walkSpeedMod;
        if (model.objectMap[ArrayHelper.ElementIndex(pos.x, pos.y,model.localSizeX)] != null)
        {
            walkSpeed *= model.objectMap[ArrayHelper.ElementIndex(pos.x, pos.y,model.localSizeX)].walkSpeedMod;
        }
        if (model.roadMap != null)
        {
            if (model.roadMap[ArrayHelper.ElementIndex(pos.x, pos.y,model.localSizeX)] != null)
            {
                walkSpeed *= model.roadMap[ArrayHelper.ElementIndex(pos.x, pos.y,model.localSizeX)].walkSpeedMod;
            }
        }
        
        Node node = new Node(walkSpeed, worldPosition);

        node.coord = pos; //set position
        return node;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}