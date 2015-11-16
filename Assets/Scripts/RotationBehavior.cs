using UnityEngine;
using System.Collections;

public class RotationBehavior : MonoBehaviour {

	public float rotationSpeed   = 20;

	public bool  rotateBehavior;
	public bool  flickerBehavior;
	public bool  growBehavior;
	public bool  randomFlickerBehavior;
	public bool  randomRotateBehavior;
	public bool  randomGrowBehavior;

	public Light spotLight;


	public float timer = 0;
	public float growingLightTimer = 0;
	
	// Use this for initialization
	void Start () 
	{
		spotLight = transform.FindChild ("Spotlight").GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (rotateBehavior) 
		{
			RotatingLightBehavior();
		}
		if (randomRotateBehavior) 
		{
			RandomRotateLightBehavior();
		}
		if (flickerBehavior) 
		{
			FlickeringLightBehavior();
		}
		if (randomFlickerBehavior) 
		{
			RandomFlickeringLightBehavior();
		}
		timer += Time.deltaTime; 
	}

	void FixedUpdate()
	{
		if (growBehavior) 
		{
			GrowLightBehavior ();
		}
		if (randomGrowBehavior) 
		{
			RandomGrowthLightBehavior();
		}
	}

	public void RotatingLightBehavior()
	{
		if ((int)timer % 2 == 0) 
		{
			transform.RotateAround (transform.position, 
		                        new Vector3 (1, 0, 0),
		                        Time.fixedDeltaTime * rotationSpeed);
		} 
		else 
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (-1, 0, 0),
			                        Time.fixedDeltaTime * rotationSpeed);
		}
	}

	public void FlickeringLightBehavior()
	{
		if ((int)timer % 3 == 0)
		{
			spotLight.intensity = 1;
		}
		else 
		{
			spotLight.intensity = 0;
		}
	}

	public void GrowLightBehavior()
	{
		growingLightTimer += Time.fixedDeltaTime;
		int growingTimerCast = (int)growingLightTimer;

		if (growingTimerCast < 1) 
		{
			spotLight.spotAngle++;
		}
		else 
		{
			spotLight.spotAngle--;

			if(spotLight.spotAngle < 20)
			{
				growingLightTimer = 0;
			}
		}

	}

	public void RandomGrowthLightBehavior()
	{
		if ((int)timer % 2 == 0) 
		{
			growingLightTimer += Time.fixedDeltaTime;
			int growingTimerCast = (int)growingLightTimer;
		
			if (growingTimerCast < 1) 
			{
				spotLight.spotAngle++;
			} else 
			{
				spotLight.spotAngle--;
			
				if (spotLight.spotAngle < 20) 
				{
					growingLightTimer = 0;
				}
			}
		}
	}

	public void RandomFlickeringLightBehavior()
	{
		if ((int)timer % 3 == 0 && Random.value > 0.5)  
		{
			spotLight.intensity = 1;
		} 
		else 
		{
			spotLight.intensity = 0;
		}
	}

	public void RandomRotateLightBehavior()
	{
		if ((int)timer % 2 == 0) 
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (0, 0, 1),
			                        Time.fixedDeltaTime * rotationSpeed);
			transform.RotateAround (transform.position, 
			                        new Vector3 (1, 0, 0),
			                        Time.fixedDeltaTime * rotationSpeed);
		} 
		else 
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (0, 0, -1),
			                        Time.fixedDeltaTime * rotationSpeed);
			transform.RotateAround (transform.position, 
			                        new Vector3 (-1, 0, 0),
			                        Time.fixedDeltaTime * rotationSpeed);

		}
	}
}
