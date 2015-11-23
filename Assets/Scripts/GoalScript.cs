﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class GoalScript : MonoBehaviour {

	public string levelToLoad;
	public Text levelCompleteText;

	bool paused = false;

	// Use this for initialization
	void Start () {
		levelCompleteText.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (paused) {
			levelCompleteText.enabled = true;
			Time.timeScale = 0;
		} else {
			levelCompleteText.enabled = false;
		}

		//if player hits start/enter while paused, move on to next level
		InputDevice device = InputManager.ActiveDevice;
		if (Input.GetKeyDown(KeyCode.Return) || device.MenuWasPressed) {
			if(paused) {
				Time.timeScale = 1;
				Application.LoadLevel(levelToLoad);
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			paused = true;
		}
	}
}
