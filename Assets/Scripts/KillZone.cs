using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

	void OnTriggerEnter(Collider coll){
		GameObject g = coll.gameObject;
		print (g.tag);
		if (g.tag == "Player") {
			g.transform.parent.gameObject.GetComponent<PlayerController>().playerCaught();
		}
	}
}
