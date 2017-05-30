using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Pathfinding : MonoBehaviour
{

    PathRequestManager requestManager;

    Grid grid;

    public Text displayFCost;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) 
    {
        Vector3[] wayp = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.WorldPointNode(startPos); 
        Node targetNode = grid.WorldPointNode(targetPos); 

        if (startNode.walkable && targetNode.walkable) // start of Pseudo Code
        {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize); // OPEN, heap used to speed up the process
            HashSet<Node> closedSet = new HashSet<Node>(); // CLOSED
            openSet.Add(startNode); // Add the start node to OPEN

            while (openSet.Count > 0) // Loop
            {
                // current = node in OPEN with the lowest f_cost
                Node currNode = openSet.RemoveFirst(); // remove current from OPEN
                closedSet.Add(currNode); // add current to CLOSED

                if (currNode == targetNode) // if current is the target node, the path has been found
                {
                    displayFCost.text = "F Cost: " + currNode.fCost;
                    pathSuccess = true;
                    break; // return
                }

                foreach (Node neighbour in grid.GetNeighbours(currNode)) // foreach neighbour of the current node
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) // if neighbour is not traversable or neighbour is in CLOSED
                    {
                        continue; // skip to the next neighbour
                    }

                    int movementCostToNeighbour = currNode.gCost + Cost(currNode, neighbour);
                    if (movementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) // if new path to neighbour is shorter OR neighbour is not in OPEN
                    {
                        // set f_cost of neighbour
                        neighbour.gCost = movementCostToNeighbour;
                        neighbour.hCost = Cost(neighbour, targetNode);
                        neighbour.parent = currNode; // set parent of neighbour to current

                        if (!openSet.Contains(neighbour)) // if neighbour is not in OPEN
                        {
                            openSet.Add(neighbour); // add neighbour to OPEN
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;

        if (pathSuccess)
        {
            wayp = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(wayp, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while(current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        Vector3[] wayp = SimplifyPath(path);
        Array.Reverse(wayp);
        return wayp;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayp = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                wayp.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return wayp.ToArray();
    }

    int Cost(Node A, Node B) // Get the distance between nodes 
    {
        int distX = Mathf.Abs(A.gridX - B.gridX);
        int distY = Mathf.Abs(A.gridY - B.gridY);

        if (distX > distY)
        {
            return (int)Math.Sqrt(Math.Pow(14 * distY, 2) + Math.Pow(10 * (distX - distY), 2));
        }
        else
        {
            return (int)Math.Sqrt(Math.Pow(14 * distX, 2) + Math.Pow(10 * (distY - distX), 2));
        }
    }
}
