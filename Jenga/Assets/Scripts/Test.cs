using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	public Material wood;
	// Use this for initialization
	
	void Start () {
		
		wood.SetFloat("_Mode", 3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
