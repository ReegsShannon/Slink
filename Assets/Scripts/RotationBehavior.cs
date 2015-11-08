using UnityEngine;
using System.Collections;

public class RotationBehavior : MonoBehaviour {

	public float rotationSpeed;
	public bool  rotateBehavior;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (rotateBehavior) 
		{
			RotatingLightBehavior();
		}
	}

	void RotatingLightBehavior()
	{
		transform.RotateAround (transform.position, 
		                        new Vector3(0,1,0),
		                        Time.fixedDeltaTime * rotationSpeed);
	}

}
