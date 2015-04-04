using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {
	static float pointerX, pointerY;
	
	public static bool swipeLeft() {
		return Input.GetKeyDown (KeyCode.A);
	}
	
	
	public static bool swipeRight() {
		return Input.GetKeyDown (KeyCode.D);
	}
	
	public static bool swipeUp() {
		return Input.GetKeyDown (KeyCode.W);
	}
	
	public static bool swipeDown() {
		return Input.GetKeyDown (KeyCode.S);
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
		} else if (Input.GetTouch (0).tapCount == 1) {
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

}
