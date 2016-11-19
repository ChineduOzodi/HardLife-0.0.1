using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

    public Transform target;

    internal Vector3 oldPosition;

    public float speed = 2;
    internal LineRenderer line;
    Node[] path;
    int targetIndex;


    LocalMapModel localMap;

	// Use this for initialization
	void Start () {

        localMap = GameObject.FindGameObjectWithTag("LocalGen").GetComponent<LocalMapController>().model;
        line = GetComponent<LineRenderer>();
        oldPosition = target.position;

    }
    void Update()
    {
        if (oldPosition != target.position)
        {
            oldPosition = target.position;
            PathRequestManager.RequestPath(transform.position - localMap.worldBottomLeft, target.position - localMap.worldBottomLeft, OnPathFound);
        }
    }
    public void OnPathFound(Node[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            //Setting LineRender vertex positions
            line.SetVertexCount(path.Length + 1);
            line.SetPosition(0, transform.position);
            for (int i = 0; i < path.Length; i++)
            {
                line.SetPosition(1 + i, path[i].worldPosition);
            }

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Node currentWaypoint = path[0];
        targetIndex = 0;
        while (true)
        {
            if (transform.position == currentWaypoint.worldPosition)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.worldPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}
