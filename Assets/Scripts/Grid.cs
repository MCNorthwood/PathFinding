using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;

    public LayerMask unwalkable;
    public Vector2 gridWorldSize;
    public float nodeRadius; // Keep to about 0.5 due to it taking up to much room otherwise
    public Node[,] matrix;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        matrix = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y /2;

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (i * nodeDiameter + nodeRadius) + Vector3.forward * (j * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkable)); // Checks for collisions to see if it is walkable
                matrix[i, j] = new Node(walkable, worldPoint, i, j); // populate the grid with nodes
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>(); // loop through x and y to find neighbours
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if(i==0 && j == 0)
                {
                    continue;
                }

                int checkX = node.gridX + i;
                int checkY = node.gridY + j;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(matrix[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node WorldPointNode(Vector3 worldPos) // changes node into a co-ord
    {
        // Find a percentage for x and y  between 0 and 1 and clamp it
        float perX = (float)(worldPos.x / gridWorldSize.x + 0.5);
        float perY = (float)(worldPos.z / gridWorldSize.y + 0.5);
        perX = Mathf.Clamp01(perX);
        perY = Mathf.Clamp01(perY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * perX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * perY);
        return matrix[x, y];
    }

    void OnDrawGizmos() // Display the grid in Unity Editor, allows to see walkable in white and unwalkable in red
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (matrix != null && displayGridGizmos)
        {
            foreach (Node node in matrix)
            {
                Gizmos.color = (node.walkable) ? Color.white : Color.black;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
