using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityStandardAssets.CrossPlatformInput;
using InControl;

public class LevelSelect : MonoBehaviour {
	
	public enum FighterOptions
	{
		ONE,
		TWO,
		THREE,
		FOUR,
		FIVE,
		SIX,
		SEVEN
	}
	
	public static LevelSelect S;
	public int                activeItem;
	public List<GameObject>   menuItems;
	public Color              highlight = Color.black;
	public float timer = 0;
	public float maxTimer = .2f;
	
	void Awake()
	{
		S = this;
	}
	
	// Use this for initialization
	void Start () 
	{
		S.gameObject.SetActive (true);
		bool first = true;
		activeItem = 0;
		
		foreach (Transform child in transform) 
		{
			menuItems.Add(child.gameObject);
		}
		
		menuItems = menuItems.OrderByDescending (m => m.transform.transform.position.y).ToList ();
		
		foreach (GameObject go in menuItems) 
		{
			GUIText itemText = go.GetComponent<GUIText>();
			if(first) itemText.color = highlight;
			first = false;
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (timer <= 0) {
			InputDevice device = InputManager.ActiveDevice;


			if (Input.GetKeyDown (KeyCode.Return) || device.Action1.WasPressed) {
				timer = maxTimer;
				switch (activeItem) {
				case (int)FighterOptions.ONE:
					Application.LoadLevel ("_level_1");
					break;
				case (int)FighterOptions.TWO:
					Application.LoadLevel ("_level_4");
					break;
				case (int)FighterOptions.THREE:
					Application.LoadLevel ("_level_5");
					break;
				case (int)FighterOptions.FOUR:
					Application.LoadLevel ("_level_6");
					break;
				case (int)FighterOptions.FIVE:
					Application.LoadLevel ("_level_6");
					break;
				}
			}
			if (Input.GetKeyDown (KeyCode.DownArrow) || device.LeftStickY < 0f) {
				timer = maxTimer;
				MoveDownMenu ();
			} else if (Input.GetKeyDown (KeyCode.UpArrow) || device.LeftStickY > 0f) {
				timer = maxTimer;
				MoveUpMenu ();
			}
		} else {
			timer -= Time.deltaTime;
		}
	}
	
	public void MoveDownMenu()
	{
		menuItems [activeItem].GetComponent<GUIText> ().color = Color.white;
		activeItem = activeItem == menuItems.Count - 1 ? 0 : ++activeItem;
		menuItems [activeItem].GetComponent<GUIText> ().color = highlight;
	}
	
	public void MoveUpMenu()
	{
		menuItems [activeItem].GetComponent<GUIText> ().color = Color.white;
		activeItem = activeItem == 0 ? menuItems.Count - 1 : --activeItem;
		menuItems [activeItem].GetComponent<GUIText> ().color = highlight;
	}
}
