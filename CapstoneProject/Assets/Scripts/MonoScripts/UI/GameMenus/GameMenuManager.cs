using UnityEngine;
using System.Collections;

public class GameMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Close this menu
	public void ResumeGame () {
		DestroyObject (this.gameObject);
	}

	// Exit to the main menu
	public void ExitToMainMenu () {
		Application.LoadLevel("MainMenu");
	}
}
