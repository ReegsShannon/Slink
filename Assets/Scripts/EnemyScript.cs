using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour {

	public List<Vector3> patrolPoints = new List<Vector3> ();
	public float patrolDist;
	public bool patrolX;
	public bool chasingPlayer = false;
	public int nextDest = 0;

	GameObject player;
	NavMeshAgent navMesh;
	PlayerController playerScript;

	Vector3 originalPosition;
	bool waiting;
	float toDest;

	// Use this for initialization
	void Start () {
		navMesh = GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerController> ();

		originalPosition = transform.position;
		navMesh.SetDestination(originalPosition);

		/*if (patrolX) {
			patrolPoints.Add (new Vector3 (originalPosition.x + patrolDist, originalPosition.y, originalPosition.z));
			patrolPoints.Add (new Vector3 (originalPosition.x - patrolDist, originalPosition.y, originalPosition.z));
		} else {
			patrolPoints.Add(new Vector3(originalPosition.x,originalPosition.y,originalPosition.z+patrolDist));
			patrolPoints.Add(new Vector3(originalPosition.x,originalPosition.y,originalPosition.z-patrolDist));
		}*/
		StartCoroutine (Patrol ());
	}
	
	// Update is called once per frame
	void Update () {
		if (!waiting && !chasingPlayer) {
			toDest = Vector3.Distance (transform.position, navMesh.destination);

			if(toDest < 0.5f)
				StartCoroutine(Patrol ());
		}
	}

	IEnumerator Patrol () {
		waiting = true;
		yield return new WaitForSeconds (2);
		GoToNextPoint ();
		waiting = false;
	}

	void GoToNextPoint() {

		if (patrolPoints.Count <= 1)
			navMesh.SetDestination (originalPosition);

		navMesh.SetDestination (patrolPoints [nextDest]);

		nextDest = (nextDest + 1) % patrolPoints.Count;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag != "Player")
			return;

		if (!playerScript.isSlinking ()) {
			navMesh.SetDestination(other.transform.position);
			chasingPlayer = true;
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag != "Player")
			return;

		if (!playerScript.isSlinking ()) {
			navMesh.SetDestination (other.transform.position);
			chasingPlayer = true;
		} else {
			chasingPlayer = false;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			navMesh.SetDestination(originalPosition);
			chasingPlayer = false;
		}
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "Player") {
			transform.position = originalPosition;
			navMesh.SetDestination(originalPosition);
			playerScript.playerCaught();
		}
	}

	public void setValues(float patrolDist_in = 0, bool patrolDir_in = true) {
		patrolDist = patrolDist_in;
		patrolX = patrolDir_in;
	}
}
