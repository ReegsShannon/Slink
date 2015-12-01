using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

	public Transform		poi;
	public Transform		camTarget;
	public GameObject	player;
	public float			distance = 7, height = 2;
	public float			u = 0.1f;
	public PlayerController behavior;

	public LayerMask mask;

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

		RaycastHit hit;
		Vector3 targetPosition = pos;
		Vector3 playerPos = player.transform.position + Vector3.up * .1f;
		if (Physics.Raycast (pos, playerPos - pos, out hit, (playerPos - pos).magnitude, mask.value)) {
			targetPosition = (hit.point - playerPos) * .8f + playerPos;
		}

		pos = targetPosition;
		transform.position = Vector3.Lerp (transform.position, pos, u);
		transform.LookAt (poi);
	}
}
