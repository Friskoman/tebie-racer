using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    public List<Transform> checkpoints; // List of checkpoints
    public Queue<Transform> priCheckpoints = new Queue<Transform>(); // Priority queue of checkpoints

	public float speed = 20;
    public float turnSpeed = 3;
	public float turnDst = 5;

    public bool walking = false;
	Path path;

	void Start() {
        // Initialize the priority queue with checkpoints
        for (int i=0; i < checkpoints.Count; i++){
            priCheckpoints.Enqueue(checkpoints[i]);
        }
	}

    void FixedUpdate () {
        if (!walking) {
            // Peek at the first checkpoint in the queue
            Transform t = priCheckpoints.Peek();
            
            // Request a path from the current position to the checkpoint
            PathRequestManager.RequestPath(new PathRequest(transform.position, t.position, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetposOld = t.position;
            if ((t.position - targetposOld).sqrMagnitude < sqrMoveThreshold) {
                // If the distance to the checkpoint is less than the move threshold, dequeue the checkpoint
                // and enqueue it back to the end of the queue
                PathRequestManager.RequestPath(new PathRequest(transform.position, t.position, OnPathFound));
                targetposOld = t.position;
                priCheckpoints.Dequeue();
                walking = true;
                priCheckpoints.Enqueue(t);
            }
        }
    }

	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path(waypoints, transform.position, turnDst);

			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath() {

        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt (path.lookPoints[0]);

		while (followingPath) {
            Vector2 pos2D = new Vector2 (transform.position.x, transform.position.z);
            while(path.turnBoundaries[pathIndex].HasCrossedLine (pos2D)) {
                if(pathIndex == path.finishLineIndex) {
                    followingPath = false;
                    walking = false;
                    break;
                }
                else {
                    pathIndex++;
                }
            }

            if (followingPath) {
                Quaternion targetRotation = Quaternion.LookRotation (path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp ( transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(speed * Time.deltaTime * Vector3.forward, Space.Self);
            }
            
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			path.DrawWithGizmos ();
		}
	}
}
