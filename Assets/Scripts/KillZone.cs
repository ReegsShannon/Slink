using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

	void OnTriggerEnter(Collider coll){
		GameObject g = coll.gameObject;
		if (g.tag == "Player") {
			print ("kill");
			g.GetComponent<PlayerController>().playerCaught();
		}
	}
}
