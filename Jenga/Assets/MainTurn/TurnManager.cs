using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour {

	public float radius = .17f;
	Vector3 towerCenter = Vector3.zero;

	float pitch =45f,
		  yaw   =0f,
		  roll  =45f;


	float degreeDelta = 45f;

	Vector3 targetPos;
	Vector3 targetRot;

	// Use this for initialization
	void Start () {
		GameObject tower = GameObject.FindGameObjectWithTag ("Tower");
		towerCenter = tower.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (TouchInput.swipeLeft())
			pitch += degreeDelta;

		if (TouchInput.swipeRight ())
			pitch -= degreeDelta;

		if (TouchInput.swipeUp ())
			roll += degreeDelta;

		if (TouchInput.swipeDown ())
			roll -= degreeDelta;

		if (TouchInput.pinchZoomOut()) {
			radius += radius*.05f;
		}
		if (TouchInput.pinchZoomIn()) {
			radius -= radius*.05f;
		}


	}

	void FixedUpdate() {

		if (roll > 360f) roll -=360f;
		if (pitch > 360f) roll -=360f;
		if (yaw > 360f) yaw -=360f;

		Vector3 targetRot = new Vector3 (roll, pitch, yaw);
		Vector3 curRot = transform.rotation.eulerAngles;
		Vector3 easedRot = Vector3.Lerp (curRot, targetRot,  .1f);
		transform.rotation = Quaternion.Euler (easedRot.x, easedRot.y, easedRot.z);


		Vector3 target = towerCenter + Quaternion.Euler (roll, pitch, yaw) * new Vector3 (0, 0, -radius);
		transform.position = Vector3.Lerp (transform.position, target, .1f);
	}
}
