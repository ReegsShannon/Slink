using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {
	
	static GameObject player;
	static Vector3 respawnSpot;

	void Awake(){
		player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null) {
			respawnSpot = player.transform.position;
		} else {
			respawnSpot = Vector3.zero;
		}
	}

	public static void respawn(){
		player.transform.position = respawnSpot;
	}
	
	void OnTriggerEnter(Collider coll){
		if(player.GetInstanceID() == coll.gameObject.GetInstanceID())
			respawnSpot = player.transform.position;
	}
}
