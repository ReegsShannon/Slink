using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using InControl;

[RequireComponent(typeof (PlayerController))]
public class UserControl : MonoBehaviour {
	private PlayerController m_Character; // A reference to the ThirdPersonCharacter on the object
	private Vector3 m_CamForward;             // The current forward direction of the camera
	private Vector3 m_Move;
	private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

	// Use this for initialization
	void Start () {
		m_Character = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		InputDevice device = InputManager.ActiveDevice;
		if (!m_Jump && (device.Action1.WasPressed || Input.GetKeyDown (KeyCode.Space))) {
			m_Jump = true;
		} 
	}

	// Fixed update is called in sync with physics
	private void FixedUpdate()
	{
		// read inputs
		float forward = 0;
		float strafe = 0;
		float rotate = 0;
		float look = 0;
		forward = Input.GetAxis("Vertical");
		strafe = Input.GetAxis("Horizontal");
		rotate = Input.GetAxis("Rotate");
		look = Input.GetAxis("Look");
		InputDevice device = InputManager.ActiveDevice;

		if (device.LeftStickX != 0f) {
			strafe = device.LeftStickX;
		}
		if (device.LeftStickY != 0f) {
			forward = device.LeftStickY;
		}
		if (device.RightStickX != 0f) {
			rotate = device.RightStickX;
		}
		if (device.RightStickY != 0f) {
			look = device.RightStickY;
		}

		float m_Rotate;
		// calculate move direction to pass to character
		m_Move = forward*Vector3.forward + strafe*Vector3.right;
		m_Rotate = rotate;


		bool hide = false;
		if (Input.GetKey (KeyCode.RightShift) || device.RightTrigger)
			hide = true;
		
		
		// pass all parameters to the character control script
		m_Character.Move(m_Move, m_Rotate, m_Jump, hide);
		m_Jump = false;

	}
}
