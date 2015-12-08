using UnityEngine;
using UnityEngine.UI;
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
	public float slinkMoveSpeed = 4f;

	GameObject playerIndicator;
	GameObject slinkIndicator;
	MeshRenderer playerRenderer;
	Collider playerCollider;
	MeshRenderer slinkRenderer;
	BoxCollider slinkCollider;

	// Sense wall
	public Transform hand;
	public float handDistance = 0.2f;
	
	// true if the character is on wall
	public bool climbing = false;

	public Transform camPoint;
	public Vector3 camLocalPos;

	public float u = 0.1f;
	public float camShiftRight = 8;
	public float camShiftForward = -8;

	public float slinkMeter = 100f;
	public float slinkRate = 25f; //rate at which slink meter is used up/regained

	//slink bar canvas objects
	public Slider slinkSlider;
	public Image fill;
	public bool onCooldown = false;
	public Color cooldownColor = Color.red;
	public Color normalColor = Color.white;

	public LayerMask mask;
	
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
		//playerIndicator = GameObject.Find ("NormalPlayer");
		//playerRenderer = playerIndicator.GetComponent<MeshRenderer> ();
		//playerCollider = playerIndicator.GetComponent<Collider> ();
		playerRenderer = GetComponent<MeshRenderer> ();
		playerCollider = GetComponent<Collider> ();
		slinkIndicator = GameObject.Find ("SlinkingPlayer");
		slinkRenderer = slinkIndicator.GetComponent<MeshRenderer> ();
		slinkCollider = slinkIndicator.GetComponent<BoxCollider> ();
		
		m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		m_OrigGroundCheckDistance = m_GroundCheckDistance;

		hand = transform;

		camPoint = GameObject.Find ("CamTarget").transform;
		camLocalPos = camPoint.localPosition;
	}

	void Update() {
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
			playerRenderer.enabled = false;
			playerCollider.enabled = false;
			slinkRenderer.enabled = true;
			slinkCollider.enabled = true;
		} else {
			slinkRenderer.enabled = false;
			slinkCollider.enabled = false;
			playerRenderer.enabled = true;
			playerCollider.enabled = true;
		}

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
			m_Rigidbody.velocity = MoveSpeed * ((transform.up * move.z) + (transform.right * move.x));
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
	
	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
		#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
		#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance,mask))
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
		CheckPoint.respawn ();
		m_Rigidbody.velocity = Vector3.zero;
	}

}
