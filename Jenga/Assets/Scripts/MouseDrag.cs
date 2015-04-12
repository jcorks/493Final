using UnityEngine;
using System.Collections;

public class MouseDrag : MonoBehaviour {

	float distance = 0.1f;

	void OnMouseDrag() {
		Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
		Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

		transform.position = objPosition;
	}

}