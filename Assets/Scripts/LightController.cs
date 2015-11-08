using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {
	
	public static LightController S;

	public Light	spotLight;
	public bool		rotateBehavior;
	public bool		flickerBehvior; 

	float timer = 0;

	void Awake()
	{
		S = this;
	}

	// Use this for initialization
	void Start () 
	{
		spotLight = gameObject.GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime; 

		if (flickerBehvior && rotateBehavior) 
		{
			FlickerAndRotationBehavior();
		}
		else if (flickerBehvior) 
		{
			FlickeringLightBehavior();
		}
		else if (rotateBehavior) 
		{
			RotatingLightBehavior();
		}

	}

	void FlickerAndRotationBehavior()
	{
		print ("Flicker and Rotation Behavior");
	}

	void FlickeringLightBehavior()
	{
		int timerCast = (int)timer;
 		
		if (timerCast % 3 == 0 && Random.value > 0.3)
		{
			spotLight.intensity = 1;
		}
		else 
		{
			spotLight.intensity = 0;
		}
	}

	void RotatingLightBehavior()
	{
		print ("Light is Rotating");
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position + Vector3.right), 10 * Time.deltaTime);
	}

}
