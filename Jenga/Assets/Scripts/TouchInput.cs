using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {
	static float pointerX, pointerY;
	static Vector2 lastpos;
	static Vector2 thispos;

	float threshold = 100f;
	float dragMinimumTimeSeconds = .07f;	

	static bool swipeLeftState = false;
	static bool swipeRightState = false;
	static bool swipeDownState = false;
	static bool swipeUpState = false;
	float time = 0f;
	
	public static bool swipeLeft() {
		return Input.GetKeyDown (KeyCode.A) || swipeLeftState;
	}
	
	
	public static bool swipeRight() {
		return Input.GetKeyDown (KeyCode.D) || swipeRightState;
	}
	
	public static bool swipeUp() {
		return Input.GetKeyDown (KeyCode.W) || swipeUpState;
	}
	
	public static bool swipeDown() {
		return Input.GetKeyDown (KeyCode.S) || swipeDownState;
	}
	
	public static bool pinchZoomIn() {
		return Input.GetKeyDown (KeyCode.Q);
	}
	
	public static bool pinchZoomOut() {
		return Input.GetKeyDown (KeyCode.E);
	}
	
	public static bool tap() {
		if (Input.GetMouseButtonDown (0)) {
			pointerX = Input.mousePosition.x;
			pointerY = Input.mousePosition.y;
			return true;
		} else if (Input.touchSupported &&    
		           Input.GetTouch (0).tapCount == 1) {
			pointerX = Input.GetTouch (0).position.x;
			pointerY = Input.GetTouch (0).position.y;
			return true;
		}

		return false;
	}
	
	public static float tapX() {
		return pointerX;
	}
	
	public static float tapY() {
		return pointerY;
	}


	void Update() {




		swipeLeftState = false;
		swipeRightState = false;
		swipeUpState = false;
		swipeDownState = false;
		time += Time.deltaTime;
		if (Input.touchSupported && Input.GetTouch (0).tapCount > 0) {
			Touch curTouch = Input.GetTouch (0);


			if (curTouch.phase == TouchPhase.Began) {
				lastpos = curTouch.position;
				time = 0f;
			} else if (curTouch.phase == TouchPhase.Moved) {
				thispos = curTouch.position;

			} else if (curTouch.phase == TouchPhase.Ended) {
				if (time < dragMinimumTimeSeconds) return;

				Vector2 delta = thispos - lastpos;

				if (delta.x < -threshold) {
					swipeLeftState = true; 
				} else if (delta.x > threshold) {
					swipeRightState = true;
				}

				if (delta.y < -threshold) {
					swipeUpState = true;
				} else if (delta.y > threshold) {
					swipeDownState = true;
				}
			}
		} 
	}
}
