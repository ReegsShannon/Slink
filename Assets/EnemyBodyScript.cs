using UnityEngine;
using System.Collections;

public class EnemyBodyScript : MonoBehaviour {

	PlayerController playerScript;

	// Use this for initialization
	void Start () {
		playerScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag != "Player") {
			return;
		} else {
			//call playerCaught function
		}
	}
}
