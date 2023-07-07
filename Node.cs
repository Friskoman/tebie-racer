using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenalty;

    public int gCost;
    public int hCost;
    public Node parent;
    private int heapIndex;

    // Gets the total cost of the node (gCost + hCost).
    public int fCost
    {
        get { return gCost + hCost; }
    }

    // Gets or sets the index of the node in the heap.
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    // Constructs a new Node instance.
    // Parameters:
    //   _walkable: Indicates if the node is walkable or blocked.
    //   _worldPos: The world position of the node.
    //   _gridX: The X coordinate of the node in the grid.
    //   _gridY: The Y coordinate of the node in the grid.
    //   _penalty: The movement penalty associated with the node.
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    // Compares this node to another node based on their fCost and hCost.
    // Parameters:
    //   nodeToCompare: The node to compare to.
    // Returns:
    //   -1 if this node has a lower fCost or hCost,
    //    0 if they are equal,
    //    1 if it is higher.
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
