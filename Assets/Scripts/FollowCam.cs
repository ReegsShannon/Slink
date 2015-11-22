using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

	public Transform		poi;
	public Transform		camTarget;
	public GameObject	player;
	public float			distance = 7, height = 2;
	public float			u = 0.1f;
	public PlayerController behavior;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		camTarget = player.transform;
		poi = GameObject.Find("CamTarget").transform;
		behavior = player.GetComponent<PlayerController> ();
	}
	
	void FixedUpdate () {
		Vector3 pos = poi.position;
		pos -= poi.forward * distance;
		pos += poi.up * height;
		
		Vector3 pos2 = (1-u)*transform.position + u*pos;
		transform.position = pos2;
		
		//transform.LookAt(camTarget);
		transform.LookAt (poi);
	}
}
