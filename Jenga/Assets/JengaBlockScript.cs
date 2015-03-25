using UnityEngine;
using System.Collections;

public class JengaBlockScript : MonoBehaviour {
	public GameObject	jengaBlockPrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Instantiate (jengaBlockPrefab);
	}
}
