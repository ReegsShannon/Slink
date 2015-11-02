using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoBehaviour {

	[SerializeField] float m_MovingTurnSpeed = 360;
	[SerializeField] float m_StationaryTurnSpeed = 180;
	[SerializeField] float m_JumpPower = 12f;
	[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
	[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
	[SerializeField] float m_MoveSpeedMultiplier = 1f;
	[SerializeField] float m_AnimSpeedMultiplier = 1f;
	[SerializeField] float m_GroundCheckDistance = 0.6f;
	[SerializeField] float m_jumpSpeed = 250;

	
	Rigidbody m_Rigidbody;
	public bool m_IsGrounded;
	float m_OrigGroundCheckDistance;
	const float k_Half = 0.5f;
	float m_TurnAmount;
	float m_ForwardAmount;
	Vector3 m_GroundNormal;
	bool m_Crouching;
	
	public int numLights = 0;
	public float slinkMoveSpeed = 6f;

	GameObject slinkIndicator;
	MeshRenderer playerRenderer;

	bool climb = false;
	
	void Start()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		playerRenderer = GetComponent<MeshRenderer> ();
		slinkIndicator = GameObject.Find ("SlinkingPlayer");
		
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
	
	public void Move(Vector3 move, bool crouch, bool jump, bool hide)
	{

		if (hide && numLights == 0) {
			slinkIndicator.GetComponent<MeshRenderer> ().enabled = true;
			playerRenderer.enabled = false;
		} else {
			slinkIndicator.GetComponent<MeshRenderer>().enabled = false;
			playerRenderer.enabled = true;
		}
		
		// convert the world relative moveInput vector into a local-relative
		// turn amount and forward amount required to head in the desired
		// direction.
		if (move.magnitude > 1f) move.Normalize();
		
		
		move = transform.InverseTransformDirection(move);
		CheckGroundStatus();
		move = Vector3.ProjectOnPlane(move, m_GroundNormal);
		float v = CrossPlatformInputManager.GetAxis ("Vertical");
		float MoveSpeed = hide ? slinkMoveSpeed : 4f;
		float RotateSpeed = 120;
		float MoveForward = Input.GetAxis("Vertical") *  MoveSpeed * Time.deltaTime;
		float MoveRotate = Input.GetAxis("Horizontal") * RotateSpeed * Time.deltaTime;
		//if (v > 0 && climb && numLights == 0 && hide) {
		if (climb && numLights == 0 && hide) {
			m_Rigidbody.useGravity = false;
			float tempZ = transform.position.z;
			transform.Translate(Vector3.up * MoveForward);
			transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * MoveSpeed * Time.deltaTime);
			var pos = transform.position;
			transform.position = new Vector3(pos.x, pos.y, tempZ);
		} else {
			m_Rigidbody.useGravity = true;
			transform.Translate(Vector3.forward * MoveForward);
			transform.Rotate(Vector3.up * MoveRotate);
			if(Input.GetAxis("Horizontal") != 0) print ("rotate");
		}
		
		if (hide && numLights == 0) {
			GetComponent<MeshRenderer>().material.color = Color.blue;
		}
		else{
			GetComponent<MeshRenderer>().material.color = Color.red;
		}

		if (!jump) 
		{
			if (Input.GetKeyDown (KeyCode.Space)) 
			{
				m_Rigidbody.AddForce(Vector3.up * m_jumpSpeed);
			}
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
}
