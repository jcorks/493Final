using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour {
	static float pointerX, pointerY;
	static Vector2 lastpos;
	static Vector2 thispos;

	float threshold = .15f;

	static bool swipeLeftState = false;
	static bool swipeRightState = false;
	static bool swipeDownState = false;
	static bool swipeUpState = false;
	static int pinchZoomState = 0;
	float time = 0f;



	static Touch touchInstance;
	static bool touchEnabled = false;	
	static bool touchUpdated = false;
	static bool swiping = false;

	void Start() {
		touchEnabled = Input.touchSupported;
	}


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
		return Input.GetKeyDown (KeyCode.Q) || pinchZoomState == -1;
	}
	
	public static bool pinchZoomOut() {
		return Input.GetKeyDown (KeyCode.E) || pinchZoomState == 1;
	}
	
	public static bool tap() {
		if (swiping)
			return false;


		if (Input.GetMouseButtonDown (0)) {
			return true;
		} else if (Input.touchSupported &&    
		           touchInstance.tapCount == 1) {
			return true;
		}

		return false;
	}
	
	public static float tapX() {
		return getTouchPos().x;
	}
	
	public static float tapY() {
		return getTouchPos ().y;
	}

	public static Vector2 tapPosition() {
		return getTouchPos();
	}


	void Update() {




		swipeLeftState = false;
		swipeRightState = false;
		swipeUpState = false;
		swipeDownState = false;
		pinchZoomState = 0;
		time += Time.deltaTime;
		UpdateTouch ();


		if (isTouchBegin()) {
			lastpos = getTouchPos();
			time = 0f;
		} else if (isTouchEnd ()) {
			thispos = getTouchPos();
			Vector2 delta = thispos - lastpos;




			if (delta.x < -threshold * Screen.width) {
				swipeLeftState = true; 
			} else if (delta.x > threshold * Screen.width) {
				swipeRightState = true;
			}

				
			if (delta.y > threshold * Screen.height) {
				swipeUpState = true;
			} else if (delta.y < -threshold * Screen.height) {
				swipeDownState = true;
				
			}
		}



	} 
	void UpdateTouch() {
		if (touchEnabled) {
			swiping = (touchInstance.phase == TouchPhase.Moved);
		} else {
		
		}
	
		/*

		if (touchEnabled && Input.GetTouch (0).tapCount > 0) {
			touchInstance = Input.GetTouch (0);

			if (Input.touchCount == 2) {

				Touch t0 = Input.GetTouch (0),
				      t1 = Input.GetTouch (1);
				Vector2 oldDelta = (t0.position - t0.deltaPosition) - 
					               (t1.position - t1.deltaPosition);
				Vector2 curDelta = t1.position - t0.position;
				if (curDelta.magnitude < oldDelta.magnitude - 20) {
					pinchZoomState = -1; 
				} else if (curDelta.magnitude < oldDelta.magnitude + 20) {
					pinchZoomState = 1;
				} else {
					pinchZoomState = 0;
				}
			}
		} 
		*/




	
	}

	bool isTouchBegin() {
		if (touchEnabled) {
			return touchInstance.phase == TouchPhase.Began;
		} 
		return Input.GetMouseButtonDown (0);
	}


	bool isTouchEnd() {
		if (touchEnabled) {
			return touchInstance.phase == TouchPhase.Ended;
		} 
		return Input.GetMouseButtonUp (0);
	}


	static Vector2 getTouchPos() {
		if (touchEnabled) {
			return touchInstance.position;
		}
		return Input.mousePosition;
	}



}
