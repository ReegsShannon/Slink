using UnityEngine;
using System.Collections;

public class SpotLightShadow : MonoBehaviour {

	public GameObject player;
	PlayerController behavior;
	float range;
	bool playerIsInSpotLight = false;

	// Use this for initialization
	void Start () {
		range = gameObject.GetComponent<Light>().range;
		behavior = player.GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
 		RaycastHit hit; 
		var lightToObject = player.transform.position - transform.position;
		var lightForward = transform.forward;
		var angleBetween = Vector3.Angle (lightToObject, lightForward);
		Debug.DrawRay(transform.position, lightToObject * 5f, Color.blue);
		if (angleBetween < GetComponent<Light>().spotAngle/2 && Physics.Raycast (transform.position, lightToObject, out hit, range)) 
		{	
 			if (hit.transform.tag == "Player") 
			{
				if (!playerIsInSpotLight) 
				{
					behavior.numLights++;
				}
				playerIsInSpotLight = true; 
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
