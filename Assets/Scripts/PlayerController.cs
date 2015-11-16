using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_GroundCheckDistance = 0.7f;
	[SerializeField] float m_jumpSpeed = 250;

	
	Rigidbody m_Rigidbody;
	public bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;

	public int numLights = 0;
	public float slinkMoveSpeed = 6f;

	GameObject slinkIndicator;
	MeshRenderer playerRenderer;
	Collider playerCollider;
	MeshRenderer slinkRenderer;
	BoxCollider slinkCollider;

	bool climb = false;
	
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		playerRenderer = GetComponent<MeshRenderer> ();
		playerCollider = GetComponent<Collider> ();
		slinkIndicator = GameObject.Find ("SlinkingPlayer");
		slinkRenderer = slinkIndicator.GetComponent<MeshRenderer> ();
		slinkCollider = slinkIndicator.GetComponent<BoxCollider> ();
		
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;
	}
	
	
	void OnCollisionEnter(Collision coll){
		if (coll.gameObject.tag == "Wall") {
			climb = true;
		}
	}
	
	void OnCollisionExit(Collision coll){
		if (coll.gameObject.tag == "Wall") {
			climb = false;
		}
	}
	
	public void Move(Vector3 move, float rotate, bool jump, bool hide)
	{

		if (hide && numLights == 0) {
			slinkRenderer.enabled = true;
			playerRenderer.enabled = false;
			slinkCollider.enabled = true;
			playerCollider.enabled = false;
		} else {
			slinkRenderer.enabled = false;
			playerRenderer.enabled = true;
			slinkCollider.enabled = false;
			playerCollider.enabled = true;
		}

		CheckGroundStatus();
		float MoveSpeed = hide ? slinkMoveSpeed : 4f;
		float RotateSpeed = 120;
		float MoveForward = move.z *  MoveSpeed * Time.deltaTime;
		float MoveRight = move.x *  MoveSpeed * Time.deltaTime;
		float MoveRotate = rotate * RotateSpeed * Time.deltaTime;
		if (climb && numLights == 0 & hide) {
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
			m_Rigidbody.useGravity = false;
			Vector3 movement = MoveSpeed * ((transform.up * move.z) + (transform.right * move.x));
			movement.z = m_Rigidbody.velocity.z;
			m_Rigidbody.velocity = movement;
		} else {
			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_Rigidbody.useGravity = true;
			Vector3 movement = MoveSpeed * ((transform.forward * move.z) + (transform.right * move.x));
			movement.y = m_Rigidbody.velocity.y;
			m_Rigidbody.velocity = movement;
			transform.Rotate(transform.up * MoveRotate);
		}
		
		if (hide && numLights == 0) {
			GetComponent<MeshRenderer>().material.color = Color.blue;
		}
		else{
			GetComponent<MeshRenderer>().material.color = Color.red;
		}

		if (jump && m_IsGrounded) 
		{
			m_Rigidbody.AddForce(Vector3.up * m_jumpSpeed);
		}


	}
	
	void HandleAirborneMovement()
	{
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		m_Rigidbody.AddForce(extraGravityForce);
		
		m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.51f;
	}
	
	
	void HandleGroundedMovement(bool crouch, bool jump)
	{
		// check whether conditions are right to allow a jump:
		if (jump && !crouch && m_IsGrounded)
		{
			// jump!
			m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
			m_IsGrounded = false;
			//m_Animator.applyRootMotion = false;
			m_GroundCheckDistance = 0.6f;
		}

	}
	
	void ApplyExtraTurnRotation()
	{
		// help the character turn faster (this is in addition to root rotation in the animation)
		float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
	}
	
	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
		#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
		#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			//m_Animator.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			m_GroundNormal = Vector3.up;
			//m_Animator.applyRootMotion = false;
		}
	}

	public bool isSlinking() {
		return slinkIndicator.GetComponent<MeshRenderer> ().enabled;
	}

	public void playerCaught() {
		//send player back to previous checkpoint
		print ("Caught");
		CheckPoint.respawn ();
	}
}
