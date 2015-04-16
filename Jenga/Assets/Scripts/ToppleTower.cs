using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class ToppleTower : MonoBehaviour {
	public AudioSource soundEffect;
	public int num_touching_table = 0;
	public bool game_over = false;
	void Update() {


	}

	void OnCollisionEnter (Collision coll) {
		// Find out what hit the table
		GameObject collidedWith = coll.gameObject;
		if (collidedWith.tag == "JengaBlock") {
			++num_touching_table;
		}
		// Once we have the concept of rounds, we can save the
		// number touching the table from the previous round
		// and check to see if the number after this round is
		// greater than the number in the previous round.
		if (num_touching_table > 3 && !game_over) {
			Debug.Log ("GAME OVER");
			soundEffect.Play();
			game_over = true;
		}
	}
}
