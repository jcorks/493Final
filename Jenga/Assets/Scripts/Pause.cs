using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour {
	public GameObject optionsCanvas;
	public GameObject pickPieceButton;
	bool was_selectable;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void pause(){
		pickPieceButton.SetActive(false);
		was_selectable = Selectable.enableSelection;
		Selectable.Freeze();
		//enable the pause menu buttons and background
		this.transform.parent.GetChild(1).gameObject.SetActive(true);
		this.transform.parent.GetChild(2).gameObject.SetActive(true);
		this.transform.parent.GetChild(3).gameObject.SetActive(true);
		this.transform.parent.GetChild(4).gameObject.SetActive(true);
	}
	
	public void resume(){
		pickPieceButton.SetActive(true);
		Selectable.enableSelection = was_selectable;
		//disable the pause menu buttons and background
		this.transform.parent.GetChild(1).gameObject.SetActive(false);
		this.transform.parent.GetChild(2).gameObject.SetActive(false);
		this.transform.parent.GetChild(3).gameObject.SetActive(false);
		this.transform.parent.GetChild(4).gameObject.SetActive(false);
	}
	
	public void openOptions(){
		//enable the options canvas
		optionsCanvas.SetActive(true);
		//disable the pause menu buttons and background
		this.transform.parent.GetChild(1).gameObject.SetActive(false);
		this.transform.parent.GetChild(2).gameObject.SetActive(false);
		this.transform.parent.GetChild(3).gameObject.SetActive(false);
		this.transform.parent.GetChild(4).gameObject.SetActive(false);
	}
	
	public void closeOptions(){
		//enable the pause menu buttons and background
		this.transform.parent.GetChild(1).gameObject.SetActive(true);
		this.transform.parent.GetChild(2).gameObject.SetActive(true);
		this.transform.parent.GetChild(3).gameObject.SetActive(true);
		this.transform.parent.GetChild(4).gameObject.SetActive(true);
		//disable the options canvas
		optionsCanvas.SetActive(false);
	}
}
