using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	static GameObject player;
	static Vector3 respawnSpot;
	static Quaternion respawnRot;
	static FollowCam cam;

	void Awake(){
		player = GameObject.Find("Player");
		cam = Camera.main.GetComponent<FollowCam> ();
		if (player != null) {
			respawnSpot = player.transform.position;
			respawnRot = player.transform.rotation;
		} else {
			respawnSpot = Vector3.zero;
		}
	}

	public static void respawn(){
		player.transform.position = respawnSpot;
		player.transform.rotation = respawnRot;
		cam.setCameraPos ();
	}

	void OnTriggerEnter(Collider coll){
		if (coll.tag == "Player") {
			respawnSpot = player.transform.position;
			respawnRot = player.transform.rotation;
		}
	}
}
