using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using InControl;

[RequireComponent(typeof (PlayerController))]
public class UserControl : MonoBehaviour {
	private PlayerController m_Character; // A reference to the ThirdPersonCharacter on the object
	private Transform m_Cam;                  // A reference to the main camera in the scenes transform
	private Vector3 m_CamForward;             // The current forward direction of the camera
	private Vector3 m_Move;
	private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

	// Use this for initialization
	void Start () {
		if (Camera.main != null)
		{
			m_Cam = Camera.main.transform;
		}
		else
		{
			Debug.LogWarning(
				"Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
			// we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
		}
		m_Character = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		InputDevice device = InputManager.ActiveDevice;
		if (!m_Jump && (device.Action1.WasPressed || Input.GetKeyDown (KeyCode.Space))) {
			m_Jump = true;
			print ("yay");
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
		forward = CrossPlatformInputManager.GetAxis("Vertical");
		strafe = CrossPlatformInputManager.GetAxis("Horizontal");
		rotate = CrossPlatformInputManager.GetAxis("Rotate");
		look = CrossPlatformInputManager.GetAxis("Look");
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
		if (m_Cam != null)
		{
			// calculate camera relative direction to move:
			//m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
			//m_Move = forward*m_CamForward + strafe*m_Cam.right;
			//m_Rotate = rotate*m_Cam.right + look*m_Cam.up;
		}
		else
		{
			// we use world-relative directions in the case of no main camera
			//m_Move = forward*Vector3.forward + strafe*Vector3.right;
		}
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
