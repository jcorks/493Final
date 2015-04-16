using UnityEngine;
using System.Collections;

public class AccelerometerInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.GetComponent<Rigidbody>().AddForce(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z, ForceMode.Acceleration);
	}
}
