﻿using UnityEngine;
using System.Collections;

public class AccelerometerInput : MonoBehaviour {
	float forceValue = 0.1f; //1/10,000
	float movingThreshold = 0.1f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 forceVector = Input.acceleration;
		if (forceVector.x < movingThreshold)
			forceVector.x = 0;
		if (forceVector.y < movingThreshold)
			forceVector.y = 0;
		if (forceVector.z < movingThreshold)
			forceVector.z = 0;
		forceVector *= forceValue;
		
		//this.gameObject.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.Acceleration);
		if (Input.GetKey(KeyCode.I)){
			this.gameObject.GetComponent<Rigidbody>().AddForce(0,0,forceValue,ForceMode.Acceleration);
		}
		if (Input.GetKey(KeyCode.J)){
			this.gameObject.GetComponent<Rigidbody>().AddForce(-forceValue,0,0,ForceMode.Acceleration);
		}
		if (Input.GetKey(KeyCode.K)){
			this.gameObject.GetComponent<Rigidbody>().AddForce(0,0,-forceValue,ForceMode.Acceleration);
		}
		if (Input.GetKey(KeyCode.L)){
			this.gameObject.GetComponent<Rigidbody>().AddForce(forceValue,0,0,ForceMode.Acceleration);
		}
	}
}
