using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class GoalScript : MonoBehaviour {

	public string levelToLoad;
	public Text levelCompleteText;
	public Text pauseText;

	bool finishedLevel = false;
	bool playerPaused = false;

	// Use this for initialization
	void Start () {
		levelCompleteText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (finishedLevel || playerPaused) {
			if(finishedLevel) { 
				levelCompleteText.enabled = true; 
			}
			else if (playerPaused) {
				pauseText.enabled = true;
			}
			Time.timeScale = 0;
		} else {
			levelCompleteText.enabled = false;
			pauseText.enabled = false;
			Time.timeScale = 1;
		}

		//if player hits start/enter while paused, move on to next level
		InputDevice device = InputManager.ActiveDevice;
		if (Input.GetKeyDown (KeyCode.Return) || device.MenuWasPressed) {
			if (finishedLevel) {
				Time.timeScale = 1;
				Application.LoadLevel (levelToLoad);
			} else {
				playerPaused = !playerPaused;
			}
		} else if (Input.GetKeyDown (KeyCode.Backspace) || device.Action4.WasPressed) {
			if (playerPaused) {
				Time.timeScale = 1;
				Application.LoadLevel ("_start_screen");
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			finishedLevel = true;
		}
	}
}
