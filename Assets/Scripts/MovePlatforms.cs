using UnityEngine;
using System.Collections;
using System.Linq;

public class MovePlatforms : MonoBehaviour {

	public float timer = 0;
	public bool zMovement = false;
	public float speed;
	public float oscillationSeconds;

	void Start()
	{

	}

	void Update()
	{
		timer += Time.deltaTime;
		if (timer < oscillationSeconds) {

			if(zMovement){
				this.transform.Translate(Vector3.forward*Time.deltaTime*speed);
			}
			else{
				this.transform.Translate(Vector3.back*Time.deltaTime*speed);
			}
		} 
		else 
		{
			timer = 0;
			if(zMovement == true){
				zMovement = false;
			}
			else{
				zMovement = true;
			}
		}
	}


}
