using UnityEngine;
using System.Collections;

public class GameOverVisual : MonoBehaviour {
	SpriteRenderer spr;
	float time = 0f;
	// Use this for initialization
	void Start () {
		spr = GetComponent<SpriteRenderer> ();
	}

	public void EnableVisual() {
		spr.enabled = true;
	}

	public void DisableVisual() {
		spr.enabled = false;
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime*2f;
		spr.transform.localScale = new Vector3 (
			.005f*(.8f + .2f*Mathf.Cos (time)),
			.005f*(.8f + .2f*Mathf.Sin (time)),
			1f);
	}
}
