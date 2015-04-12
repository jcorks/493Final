using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void loadOptions(){
		Application.LoadLevel("OptionsMenu");
	}
	
	public void loadMainMenu(){
		Application.LoadLevel("MainMenu");
	}
	
	public void loadGame(){
		Application.LoadLevel("jc_scene");
	}
	
	public void loadQuit(){
		Application.Quit();
	}
}
