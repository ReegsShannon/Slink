using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

	[SerializeField] float m_GroundCheckDistance = 0.2f;
	[SerializeField] float m_jumpSpeed = 250;

	Rigidbody m_Rigidbody;
	public bool m_IsGrounded;

	public int numLights = 0;
	public float slinkMoveSpeed = 4f;

	// Sense wall
	Transform hand;
	public float handDistance = 0.3f;
	
	// true if the character is on wall
	public bool climbing = false;

	public Transform camPoint;
	public Vector3 camLocalPos;

	public float u = 0.1f;
	public float camShiftRight = 2;
	public float camShiftForward = -2;

	public float slinkMeter = 100f;
	public float slinkRate = 25f; //rate at which slink meter is used up/regained

	//slink bar canvas objects
	public Slider slinkSlider;
	public Image fill;
	public bool onCooldown = false;
	public Color cooldownColor = Color.red;
	public Color normalColor = Color.blue;

	public LayerMask mask;

	Transform meshTransform;
	public float shrinkSpeed = 6f;
	public float shrinkScale = .01f;
	
	// detect raycat hit
	void DetectHit(ref RaycastHit detectedHit, Transform transform)
	{
		RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, handDistance);
		foreach (RaycastHit hit in hits)
		{
			if (hit.collider == GetComponent<Collider>())
				continue;
			if (hit.collider.isTrigger)
				continue;
			if (detectedHit.collider == null || hit.distance < detectedHit.distance)
				detectedHit = hit;
		}
	}
	
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		meshTransform = GameObject.Find ("NormalPlayer").transform;
		
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

		hand = transform;

		camPoint = GameObject.Find ("CamTarget").transform;
		camLocalPos = camPoint.localPosition;
	}

	void Update() {
		bool shrinking = false;
		if (Input.GetKeyDown (KeyCode.R)) {
			CheckPoint.respawn();
			m_Rigidbody.velocity = Vector3.zero;
		}

		if (isSlinking ()) {
			slinkMeter -= slinkRate * Time.deltaTime;
			if (slinkMeter < 0f) {
				slinkMeter = 0f;
				onCooldown = true;
				//set color of bar to red
				fill.color = cooldownColor;
			}
		} else {
			slinkMeter += slinkRate * Time.deltaTime;
			if (slinkMeter > 100f) {
				slinkMeter = 100f;
			} else if(slinkMeter > 25f && onCooldown) {
				onCooldown = false;
				//re-set color of bar to gray
				fill.color = normalColor;
			}
		}
		print (fill.color.ToString());
		print (normalColor.ToString());
		slinkSlider.value = slinkMeter;
	}
	
	public void Move(Vector3 move, float rotate, bool jump, bool hide)
	{
		CheckGroundStatus();
		RaycastHit handHit = new RaycastHit();
		DetectHit(ref handHit, hand);
		bool slink = false;
		if(hide && numLights == 0 && !onCooldown && (m_IsGrounded || handHit.collider != null)) slink = true;

		//if slink meter is empty, disable slinking
		if (slinkMeter <= 0)
			slink = false;

		if (slink) {
			if (meshTransform.localScale.y < shrinkScale){
				meshTransform.localScale = new Vector3(1, shrinkScale, 1);
			}else if(meshTransform.localScale.y > shrinkScale){
				meshTransform.localScale -= new Vector3(0,1,0) * Time.deltaTime * shrinkSpeed;
				if (meshTransform.localScale.y < shrinkScale)
					meshTransform.localScale = new Vector3(1, shrinkScale, 1);
			}
		} else {
			if (meshTransform.localScale.y < 1){
				meshTransform.localScale += new Vector3(0,1,0) * Time.deltaTime * shrinkSpeed;
				if(meshTransform.localScale.y > 1)
					meshTransform.localScale = new Vector3(1,1,1);
			}
			else if(meshTransform.localScale.y > 1)
				meshTransform.localScale = new Vector3(1,1,1);
		}

		if (!Mathf.Approximately (meshTransform.localScale.y, shrinkScale))
			slink = false;

		transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y,0);

		float MoveSpeed = slink ? slinkMoveSpeed : 3f;
		float RotateSpeed = 120;
		float MoveRotate = rotate * RotateSpeed * Time.deltaTime;

		// detect new wall for climbling
		if (!climbing && handHit.collider != null && slink){
			climbing = true;
			m_Rigidbody.useGravity = false;
		}
		// no wall or stop climbing
		else if (climbing && handHit.collider == null || !slink){
			this.climbing = false;
			m_Rigidbody.useGravity = true;
		}
		
		// Jump
		if (jump && m_IsGrounded) {
			m_Rigidbody.AddForce(Vector3.up * m_jumpSpeed);
			climbing = false;
			m_Rigidbody.useGravity = true;
		}
		
		// climbling action.
		if (climbing){
			float moveZ = move.z;
			float moveX = move.x;
			float newSpeed;
			if(Mathf.Abs(moveZ) > Mathf.Abs(moveX)){
				newSpeed = Mathf.Abs (move.z) * MoveSpeed;
			}else{
				newSpeed = Mathf.Abs (move.x) * MoveSpeed;
			}


			m_Rigidbody.velocity = newSpeed * Vector3.Normalize(((transform.up * move.z) + (transform.right * move.x)));
			m_Rigidbody.angularVelocity = Vector3.zero;
			transform.rotation = Quaternion.LookRotation(handHit.normal*-1f);

			if(move.x > .2f){
				Vector3 pos = camLocalPos + Vector3.right * camShiftRight + Vector3.forward * camShiftForward;
				Vector3 pos2 = (1-u)*camPoint.localPosition + u*pos;
				camPoint.localPosition = pos2;
			}
			else if(move.x < -.2f){
				Vector3 pos = camLocalPos - Vector3.right * camShiftRight + Vector3.forward * camShiftForward;
				Vector3 pos2 = (1-u)*camPoint.localPosition + u*pos;
				camPoint.localPosition = pos2;
			}else{
				Vector3 pos = camLocalPos + Vector3.forward * camShiftForward;
				Vector3 pos2 = (1-u)*camPoint.localPosition + u*pos;
				camPoint.localPosition = pos2;
			}

		}
		// walking or jump action
		else{
			float u = 0.1f;
			Vector3 pos = camLocalPos;
			Vector3 pos2 = (1-u)*camPoint.localPosition + u*pos;
			camPoint.localPosition = pos2;
			Vector3 movement = MoveSpeed * ((transform.forward * move.z) + (transform.right * move.x));
			movement.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = movement;
			transform.Rotate(transform.up * MoveRotate);
		}

	}
	
	void CheckGroundStatus(){
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance,mask)){
			m_IsGrounded = true;
		}
		else{
			m_IsGrounded = false;
		}
	}

	public bool isSlinking() {
		return Mathf.Approximately (meshTransform.localScale.y, shrinkScale);
	}

	public void playerCaught() {
		//send player back to previous checkpoint
		CheckPoint.respawn ();
		m_Rigidbody.velocity = Vector3.zero;
	}

}
