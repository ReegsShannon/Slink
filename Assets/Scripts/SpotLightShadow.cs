using UnityEngine;
using System.Collections;

public class SpotLightShadow : MonoBehaviour {

	public GameObject player;
	PlayerController behavior;
	float range;
	bool playerIsInSpotLight = false;

	public Vector3 lightToObject;
	public Vector3 lightForward;
	public float angleBetween = 0;

	// Use this for initialization
	void Start () {
		range = gameObject.GetComponent<Light>().range;
		behavior = player.GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
 		RaycastHit hit; 
		lightToObject = player.transform.position - transform.position;
		lightForward = transform.forward;
		angleBetween = Vector3.Angle (lightToObject, lightForward);
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
