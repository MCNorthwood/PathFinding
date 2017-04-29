using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour {

    PathRequestManager requestManager;

    Grid grid;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) // Based on Pseudo Code for A*
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos); // Open Set
        Node targetNode = grid.NodeFromWorldPoint(targetPos); // Closed Set

        if (startNode.walkable && targetNode.walkable)
        {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode); // Add the start node to OPEN

            while (openSet.Count > 0) // Loop
            {
                // current = node in OPEN with the lowest f_cost
                Node currentNode = openSet.RemoveFirst(); // remove current from OPEN
                closedSet.Add(currentNode); // add current to CLOSED

                if (currentNode == targetNode) // if current is the target node //path has been found
                {
                    pathSuccess = true;
                    break; // return
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode)) // foreach neighbour of the current node
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) // if neighbour is not traversable or neighbour is in CLOSED
                    {
                        continue; // skip to the next neighbour
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) // if new path to neighbour is shorter OR neighbour is not in OPEN
                    {
                        // set f_cost of neighbour
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode; // set parent of neighbour to current

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
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
