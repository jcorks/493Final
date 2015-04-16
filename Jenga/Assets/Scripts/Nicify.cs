using UnityEngine;
using System.Collections;

public class Nicify : MonoBehaviour {
	float time;
	Color col;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > 1) {
			time = 0;
			col = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 1);
		}
		GetComponent<TextMesh> ().color = Color.Lerp (GetComponent<TextMesh> ().color, col, .05f);
	}
}
