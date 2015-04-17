using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FinalScoreDisplay : MonoBehaviour {
	Text score_display;
	
	// Use this for initialization
	void Start () {
		score_display = this.gameObject.GetComponentInChildren<Text>();
		int score = Scoreholder.finalScore;
		score_display.text = "Your score: " + score.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
