using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scoreholder : MonoBehaviour {
	public static int finalScore;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static void setScore(int score_in){
		finalScore = score_in;
	}
	
	public int getScore(){
		return finalScore;
	}
	
	public void saveScore(){
		Text name = GameObject.Find("NameInput").GetComponent<Text>();
		if (name.text != ""){
			HighScoreManager._instance.SaveHighScore(name.text, finalScore);
			GameObject load = GameObject.Find("LoadScene");
			load.GetComponent<LoadScene>().loadMainMenu();
		}
	}

}
