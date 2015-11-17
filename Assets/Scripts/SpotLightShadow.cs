using UnityEngine;
using System.Collections;

public class SpotLightShadow : MonoBehaviour {

	public GameObject player;
	PlayerController behavior;
	float range;
	bool playerIsInSpotLight = false;
	float spotAngle;
	float playerHeight;

	public LayerMask mask = -1;

	// Use this for initialization
	void Start () {
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
		//print (angleBetween + " : " + GetComponent<Light>().spotAngle/2);
		//Debug.DrawRay(transform.position, lightToObject * range, Color.blue);
		Debug.DrawLine(transform.position, player.transform.position, Color.blue);
		if (angleBetween < spotAngle/2 && Physics.Raycast (transform.position, lightToObject, out hit, range, mask.value)){	
			print ("in Light: " + gameObject.GetInstanceID());
			if (hit.transform.tag == "Player"){
				if (!playerIsInSpotLight) {
					behavior.numLights++;
				}
				playerIsInSpotLight = true; 
			} 
			else{
				print ("blocked!: " + gameObject.GetInstanceID() + " " + hit.transform.name);
				if (playerIsInSpotLight){
					behavior.numLights--;
				}
				playerIsInSpotLight = false; 
			}
		} 
		else
		{
			print ("not in Light: " + gameObject.GetInstanceID());
			if (playerIsInSpotLight)
			{
				behavior.numLights--;
			}
			playerIsInSpotLight = false; 
		}
	}
}
