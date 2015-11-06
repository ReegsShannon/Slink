using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	GameObject player;
	NavMeshAgent navMesh;
	PlayerController playerScript;

	Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		navMesh = GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerController> ();

		originalPosition = transform.position;
		navMesh.SetDestination(originalPosition);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag != "Player")
			return;

		if (!playerScript.isSlinking ()) {
			navMesh.SetDestination(other.transform.position);
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag != "Player")
			return;

		if (!playerScript.isSlinking ()) {
			navMesh.SetDestination (other.transform.position);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			navMesh.SetDestination(originalPosition);
		}
	}
}
