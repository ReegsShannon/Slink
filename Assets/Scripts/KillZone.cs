using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

	void OnTriggerEnter(Collider coll){
		GameObject g = coll.gameObject;
		print (g.tag);
		if (g.tag == "Player") {
			g.GetComponent<PlayerController>().playerCaught();
		}
		else if(g.tag == "PlayerSlink" || g.tag == "PlayerNormal"){
			g.transform.parent.gameObject.GetComponent<PlayerController>().playerCaught();
		}
	}
}
