using UnityEngine;

public class MoveObject : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    float minX;
    float maxX;
    float minZ;
    float maxZ;

    Grid grid;
    public GameObject aStar;

    TrailRenderer trail;

    private void Start()
    {
        grid = aStar.GetComponent<Grid>();
        trail = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        minX = (grid.gridWorldSize.x / 2) - grid.gridWorldSize.x;
        maxX = grid.gridWorldSize.x / 2;
        minZ = (grid.gridWorldSize.y / 2) - grid.gridWorldSize.y;
        maxZ = grid.gridWorldSize.y / 2;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;

        transform.position = cursorPosition;
        
        if (trail != null)
        {
            trail.Clear();
        }
    }
}
