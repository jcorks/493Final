using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ScoreType{
	name,
	score
}

public class PopulateHighscoreList : MonoBehaviour {
	public ScoreType type;
	Text info;
	
	// Use this for initialization
	void Start () {
		info = GetComponent<Text>();
		List<Scores> names_and_scores = HighScoreManager._instance.GetHighScore();
		foreach (Scores scores_pair in names_and_scores){
			if (type == ScoreType.name)
				info.text += "\n" + scores_pair.name;
			else 
				info.text += "\n" + scores_pair.score;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void clearList(){
		if (type == ScoreType.name){
			info.text = "Name\n";
		}
		else
			info.text = "Score\n";
	}
}
