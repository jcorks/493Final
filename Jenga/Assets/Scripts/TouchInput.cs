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

	
	static Vector3 lastPointer;
	static Vector3 pointerDelta;


	// Device specific stuff
	static bool android_device = false;
	static bool ios_device = false;
	static bool probably_a_computer = false;
	void Awake() {
		if (Application.platform == RuntimePlatform.Android) {
			Debug.Log ("It's Android!");
			android_device = true;
		} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			Debug.Log ("It's an iPhone!");
			ios_device = true;
		} else {
			Debug.Log ("It's Something Else!");
			probably_a_computer = true;
		}
	}

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

		if (Input.GetMouseButton(0)) {
			return true;
		} else if (touchEnabled) {    
		           //touchInstance.tapCount == 1) {
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

		pointerDelta = Input.mousePosition - lastPointer;
		lastPointer = Input.mousePosition;


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
			if (Input.touchCount >= 1) {
				touchInstance = Input.GetTouch (0);
				Debug.Log("UpdateTouch()");
			}

			/*
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
			*/
		} 





	
	}

	bool isTouchBegin() {
		if (touchEnabled) {
			if (ios_device && Input.touchCount == 0) {
				Debug.Log("isTouchBegin() *FALSE");
				//return false;
			} else if (ios_device && Input.touchCount >= 1) {
				Debug.Log("isTouchBegin() TRUE");
			}
			return touchInstance.phase == TouchPhase.Began;
		} 
		return Input.GetMouseButtonDown (0);
	}


	bool isTouchEnd() {
		if (touchEnabled) {
			if (touchInstance.phase == TouchPhase.Ended) {
				Debug.Log("isTouchEnd()");
			}
			return touchInstance.phase == TouchPhase.Ended;
		} 
		return Input.GetMouseButtonUp (0);
	}


	static Vector2 getTouchPos() {
		if (touchEnabled) {
			if (ios_device && Input.touchCount == 0) {
				Debug.Log("getTouchPos() Nothing touching screen");
			//	return Vector3.zero;
			} else if (ios_device && Input.touchCount >= 1) {
				Debug.Log("getTouchPos() "+Input.touchCount+" fingers on the screen");
				return touchInstance.position;
			}
			return touchInstance.position;
		}
		return Input.mousePosition;
	}


	static public Vector3 tapDelta() {
		if (touchEnabled) {

			if (ios_device && Input.touchCount == 0) {
				return Vector3.zero;
				Debug.Log("tapDelta() nothing touching!");
			} else if (ios_device && Input.touchCount >= 1) {
				Debug.Log("tapDelta() "+Input.touchCount+" things touching");
			}
			return touchInstance.deltaPosition;
		} else {
			return pointerDelta;
		}
	}


}
