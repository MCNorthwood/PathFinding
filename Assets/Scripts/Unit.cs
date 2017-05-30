using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    float speed = 30;
    Vector3[] path;
    int targetIndex;
    public bool clicked = false;

    Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        if (clicked)    // for scene 3 to start all of them at once.
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }
    }

    public void StartPath()
    {
        if (!clicked)
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
            clicked = true;
            canvas.gameObject.SetActive(false);
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime); //move to next waypoint
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for(int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
