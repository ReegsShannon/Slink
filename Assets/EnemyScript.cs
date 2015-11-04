using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	NavMeshAgent navMesh;
	PlayerController playerScript;

	// Use this for initialization
	void Start () {
		navMesh = GetComponent<NavMeshAgent> ();
		playerScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();

		navMesh.SetDestination(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag != "Player")
			return;

		/*if (playerScript.isHidden()) {
			set destination to player
		}*/
	}
}
