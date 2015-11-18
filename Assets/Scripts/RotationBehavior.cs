using UnityEngine;
using System.Collections;
using System;

public class RotationBehavior : MonoBehaviour {

	public float rotationSpeed   = 20;

	public bool  rotateYBehavior;
	public bool  rotateXBehavior;
	public bool  rotateZBehavior;
	public bool  flickerBehavior;
	public bool  growBehavior;
	public bool  randomFlickerBehavior;
	public bool  randomRotateBehavior;
	public bool  randomGrowBehavior;

	public Light spotLight;


	public float timer = 0;
	public float growingLightTimer = 0;

	public float timeLength = 2;

	bool increasing = true;
	
	// Use this for initialization
	void Start () 
	{
		spotLight = transform.FindChild ("Spotlight").GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Math.Abs (timer) > timeLength / 2) {
			increasing = !increasing;
			timer = Math.Sign(timer) * (timeLength/2);
		}
		if (increasing)
			timer += Time.deltaTime;
		else 
			timer -= Time.deltaTime;
		if (rotateYBehavior) 
		{
			RotatingLightYBehavior();
		}
		if (rotateXBehavior) 
		{
			RotatingLightXBehavior();
		}
		if (rotateZBehavior) 
		{
			RotatingLightZBehavior();
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

	public void RotatingLightYBehavior()
	{
		if (increasing) 
		{
			transform.RotateAround (transform.position, 
		                        new Vector3 (1, 0, 0),
		                        Time.deltaTime * rotationSpeed);
		} 
		else 
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (-1, 0, 0),
			                        Time.deltaTime * rotationSpeed);
		}
	}

	public void RotatingLightZBehavior()
	{
		if (increasing)  
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (0, 1, 0),
			                        Time.deltaTime * rotationSpeed);
		} 
		else 
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (0, -1, 0),
			                        Time.deltaTime * rotationSpeed);
		}
	}

	public void RotatingLightXBehavior()
	{
		if (increasing)  
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (0, 0, 1),
			                        Time.deltaTime * rotationSpeed);
		} 
		else 
		{
			transform.RotateAround (transform.position, 
			                        new Vector3 (0, 0, -1),
			                        Time.deltaTime * rotationSpeed);
		}
	}

	public void FlickeringLightBehavior()
	{
		if (increasing) 
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
		if (increasing) 
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
		if (increasing && UnityEngine.Random.value > 0.5)  
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
		if (increasing) 
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
