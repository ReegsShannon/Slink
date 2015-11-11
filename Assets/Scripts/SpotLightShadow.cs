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
		var rayDirection = player.transform.position - range*transform.position;
		Debug.DrawRay(transform.position, rayDirection * 5f, Color.blue);
		if (Physics.Raycast (transform.position, rayDirection, out hit)) 
		{	
 			if (hit.transform.tag == "Player") 
			{
				if (!playerIsInSpotLight) 
				{
					behavior.numLights++;
				}
				playerIsInSpotLight = true; 
				print("IN THE LIGHT");

			} 
			else 
			{
				if (playerIsInSpotLight) 
				{
					behavior.numLights--;
				}
				playerIsInSpotLight = false; 
				print ("OUT OF THE LIGHT");
			}
		} 
		else 
		{
			if (playerIsInSpotLight)
			{
				behavior.numLights--;
			}
			playerIsInSpotLight = false; 
			print ("OUT OF THE LIGHT");
		}
	}
}
