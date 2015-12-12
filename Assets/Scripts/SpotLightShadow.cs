using UnityEngine;
using System.Collections;

public class SpotLightShadow : MonoBehaviour {

	public GameObject player;
	PlayerController behavior;
	float range;
	bool playerIsInSpotLight = false;
	float spotAngle;
	float playerHeight;

	public LayerMask mask;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		range = gameObject.GetComponent<Light>().range;
		behavior = player.GetComponent<PlayerController> ();
		spotAngle = GetComponent<Light> ().spotAngle;
	}
	
	// Update is called once per frame
	void Update () {
 		RaycastHit hit; 
		var lightToObject = player.transform.position - transform.position;
		var lightForward = transform.forward;
		var angleBetween = Vector3.Angle (lightToObject, lightForward);
		//Debug.DrawLine(transform.position, player.transform.position, Color.blue);
		//Debug.DrawLine(transform.position, player.transform.position + Vector3.up, Color.blue);
		if (angleBetween < spotAngle/2 && Physics.Raycast (transform.position, lightToObject, out hit, range, mask.value)){	
			if (hit.transform.tag == "Player"){
				if (!playerIsInSpotLight) {
					behavior.numLights++;
				}
				playerIsInSpotLight = true; 
			} 
			else{
				if (playerIsInSpotLight){
					behavior.numLights--;
				}
				playerIsInSpotLight = false; 
			}
		} 
		else
		{
			if (playerIsInSpotLight)
			{
				behavior.numLights--;
			}
			playerIsInSpotLight = false; 
		}
	}
}
