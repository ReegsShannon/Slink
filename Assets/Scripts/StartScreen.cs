using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StartScreen : MonoBehaviour {

	public enum FighterOptions
	{
		STARTGAME,
		LEVELSELECT,
		QUITGAME
	}
	
	public static StartScreen S;
	public int                activeItem;
	public List<GameObject>   menuItems;
	public Color              highlight = Color.black;
	
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
		if (Input.GetKeyDown (KeyCode.Return)) 
		{
			switch(activeItem)
			{
			case (int)FighterOptions.STARTGAME:
				Application.LoadLevel("_level_1");
				break;
			case (int)FighterOptions.LEVELSELECT:
				print ("level select");
				break;
			case (int)FighterOptions.QUITGAME:
				Application.Quit();
				print ("quit game");
				break;
			
			}
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			MoveDownMenu ();
		} 
		else if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			MoveUpMenu();
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
