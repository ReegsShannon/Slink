using UnityEngine;
using System.Collections;

public class PlayerShadow : MonoBehaviour {
	public GameObject player;
	UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter behavior;
	float range;
	bool playerIsInLight = false;

	// Use this for initialization
	void Start () {
		range = gameObject.GetComponent<Light>().range;
		behavior = player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter> ();
	}
	
	// Update is called once per frame
	void Update () {
		//float dist = Vector3.Distance(player.transform.position, this.transform.position); 
		RaycastHit hit; 
		var rayDirection = player.transform.position - transform.position;
		Debug.DrawRay(transform.position, rayDirection * 5f, Color.magenta);
		if (Physics.Raycast (transform.position, rayDirection, out hit)) {	
		//if (Physics.Raycast (transform.position, rayDirection, out hit, range)) {
			if (hit.transform.tag == "Player") {
				if (!playerIsInLight) {
					behavior.numLights++;
				}
				playerIsInLight = true; 
			} else {
				if (playerIsInLight) {
					behavior.numLights--;
				}
				playerIsInLight = false; 
			}
		} else {
			if (playerIsInLight) {
				behavior.numLights--;
			}
			playerIsInLight = false; 
		}
	}
}
