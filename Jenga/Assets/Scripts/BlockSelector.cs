using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Gives the ability for the user to tap a Jenga block and select it */

public class BlockSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (TouchInput.tap ()) {
			RaycastHit hitInfo;
			Ray hit = Camera.main.ScreenPointToRay(new Vector3(TouchInput.tapX(), TouchInput.tapY (), 0));

			if (Physics.Raycast(hit, out hitInfo)) {
				if (hitInfo.collider.gameObject.tag == "JengaBlock") {
					hitInfo.collider.gameObject.GetComponent<Selectable>().Select();

				}
			}
		}
	}
}
