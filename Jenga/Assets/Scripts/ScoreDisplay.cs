using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {
	public bool totalScore;
	TurnManager turnMngr;
	Text display;
	
	// Use this for initialization
	void Start () {
		turnMngr = GameObject.Find("JengaView").GetComponent<TurnManager>();
		display = this.gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if (totalScore)
			display.text = "Total: " + turnMngr.totalScore.ToString();
		else
			display.text = "Turn: " + turnMngr.turnScore.ToString();
	}
}
