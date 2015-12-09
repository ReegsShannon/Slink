using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour {

	public List<Vector3> patrolPoints = new List<Vector3> ();
	public GameObject[] allEnemies;
	public float waitTime = 2f;
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
		allEnemies = GameObject.FindGameObjectsWithTag ("Enemy");

		originalPosition = transform.position;
		navMesh.SetDestination(originalPosition);

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
		yield return new WaitForSeconds (waitTime);
		GoToNextPoint ();
		waiting = false;
	}

	void GoToNextPoint() {

		if (patrolPoints.Count <= 1) {
			navMesh.SetDestination (originalPosition);
			return;
		}

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

	public void resetEnemy() {
		transform.position = originalPosition;
		navMesh.SetDestination (originalPosition);
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "Player") {
			foreach (GameObject enemy in allEnemies) {
				enemy.GetComponent<EnemyScript>().resetEnemy();
			}
			playerScript.playerCaught();
		}
	}
}
