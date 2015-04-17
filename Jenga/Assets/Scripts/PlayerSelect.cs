using UnityEngine;
using System.Collections;

public class PlayerSelect : MonoBehaviour {
	public static int num_players;
	
	// Use this for initialization
	void Start () {
		num_players = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void setNumPlayers(int num){
		num_players = num;
	}
}
