using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

	public enum TurnPhase
	{
		InitialPhase,
		ChoosePiece,
		DragPiece,
		ReplacePiece,
		FreeMode

	};


	public float radius = .17f;
	public float degreeDelta = 45f;
	public float radiusDeltaRatio = .5f;
	public Vector3 offset = Vector3.zero;
	Vector3 towerCenter = Vector3.zero;

	float pitch =45f,
		  yaw   =0f,
		  roll  =45f;




	Vector3 targetPos;
	Vector3 targetRot;


	Vector3 selectedOriginalPosition;
	Vector3 selectedOriginalRotation;


	bool hasStartedDragUpdate = false;
	bool hasStartedChooseUpdate = false;
	bool hasStartedDragging = false;
	GameObject gameButton;

	Vector2 dragPos;
	Vector2 pointerDelta;
	Vector2 lastPointer;

	float dragTimer = 0f;
	public Material DragMaterial;

	TurnPhase phase = TurnPhase.ChoosePiece;

	// Use this for initialization
	void Start () {
		gameButton = GameObject.FindObjectOfType<Button> ().gameObject;
		GameObject tower = GameObject.FindGameObjectWithTag ("Tower");
		if (tower == null) {
			Debug.Log ("WARNING! TurnManager could not find the tower prefab to center itself!");
		} else {
			towerCenter = tower.transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (phase) {
		case TurnPhase.InitialPhase:
			InitialPhaseUpdate();
			break;
		case TurnPhase.ChoosePiece:
			ChoosePieceUpdate ();
			break;
		case TurnPhase.DragPiece:
			DragPieceUpdate ();
			break;
		}


	}



	void FixedUpdate() {

		pointerDelta = new Vector2 (Input.mousePosition.x - lastPointer.x,
		                            Input.mousePosition.y - lastPointer.y); 
		lastPointer = Input.mousePosition;

		// First bind values to 0 - 360 to prevent false lerping


		if (roll >= 360f + transform.eulerAngles.x) roll -=360f;
		if (pitch >= 360f + transform.eulerAngles.y) pitch -=360f;
		if (yaw >= 360f + transform.eulerAngles.z) yaw -=360f;

		if (roll <= -360f + transform.eulerAngles.x) roll +=360f;
		if (pitch <= -360f + transform.eulerAngles.y) pitch +=360f;
		if (yaw <= -360f + transform.eulerAngles.z) yaw +=360f;


		// Then ease BOTH angle rotation and position movement
		Vector3 targetRot = new Vector3 (roll, pitch, yaw);
		Vector3 curRot = transform.rotation.eulerAngles;
		Vector3 easedRot = Vector3.Lerp (curRot, targetRot,  .1f);
		transform.rotation = Quaternion.Euler (easedRot.x, easedRot.y, easedRot.z);



		transform.position = Vector3.Lerp (transform.position, targetPos, .1f);

	}


	
	void InitialPhaseUpdate() {
		changePhase(TurnPhase.ChoosePiece);
	}
	
	
	void ChoosePieceUpdate() {
		if (!hasStartedChooseUpdate) {
			gameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Pick Piece";
			var buttonCallback = new Button.ButtonClickedEvent();
			buttonCallback.AddListener(FinalizePiece);
			gameButton.GetComponent<Button>().onClick = buttonCallback;
			Selectable.Thaw ();
			transform.rotation = Quaternion.Euler (new Vector3 (roll, pitch, yaw));
			hasStartedChooseUpdate = true;
			Selectable.Deselect();
		}

		if (TouchInput.swipeLeft())
			pitch -= degreeDelta;
		
		if (TouchInput.swipeRight ()) 
			pitch += degreeDelta;
		
		
		if (TouchInput.swipeUp ()) {
			if (roll == 0) return;
		    roll -= degreeDelta;

		}
		
		if (TouchInput.swipeDown ()) {
			if (roll == 90) return;
			roll += degreeDelta;

		}
		
		if (TouchInput.pinchZoomOut()) {
			radius += radius*radiusDeltaRatio;
		}
		if (TouchInput.pinchZoomIn()) {
			radius -= radius*radiusDeltaRatio;
		}

		targetPos = offset + towerCenter + Quaternion.Euler (roll, pitch, yaw) * new Vector3 (0, 0, -radius);
	}


	void DragPieceUpdate() {
		GameObject piece = Selectable.GetSelection();
		if (!hasStartedDragUpdate) {
			selectedOriginalPosition = piece.transform.position;
			selectedOriginalRotation = piece.transform.rotation.eulerAngles;
			hasStartedDragUpdate = true;
			Selectable.Freeze ();
			gameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Back";
			var buttonCallback = new Button.ButtonClickedEvent();
			buttonCallback.AddListener(UndoPiece);
			gameButton.GetComponent<Button>().onClick = buttonCallback; 
			dragTimer = 0f;


		}
		dragTimer += Time.deltaTime;

		Vector3 pieceRot = selectedOriginalRotation;
		targetPos = selectedOriginalPosition + 
			Quaternion.Euler (pieceRot.x, pieceRot.y - 90, pieceRot.z) * new Vector3 (0, 0, -1f*radius);
		transform.LookAt (selectedOriginalPosition);


		if (dragTimer > 1.4f) {
			if (dragTimer < 1.8f) {
				dragPos = Camera.main.WorldToScreenPoint(piece.transform.position);
				piece.GetComponent<MeshRenderer>().material = DragMaterial;
			}
			if (Input.GetMouseButton(0))
				UserDragPiece ();
		}
	}

	// Takes the currently selected piece and prepares it for movement.
	public void FinalizePiece() {
		changePhase(TurnPhase.DragPiece);
		//Destroy(Selectable.GetSelection ());
	}

	public void UndoPiece() {
		changePhase ( TurnPhase.ChoosePiece);
	}


	void changePhase(TurnPhase p) {
		hasStartedDragUpdate = false;
		hasStartedChooseUpdate = false;
		phase = p;
	}



	// Put logic here for dragging the piece
	void UserDragPiece() {
		dragPos += pointerDelta;

		// Get the z coordinate of the piece you wanna drag
		GameObject piece = Selectable.GetSelection();
		var dir = piece.GetComponent<JengaBlockScript> ().direction;
		//Debug.Log ("this piece's rotation: " + rotation);

		if (dir == JengaBlockScript.Direction.FacingSouth || dir == JengaBlockScript.Direction.FacingNorth) {
			var original_z = piece.transform.position.z;
			Vector3 mousePos2D = dragPos;
			mousePos2D.z = original_z; // fix the z coordinate when viewing this face
			Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
			Vector3 pos = this.transform.position;
			pos.x = mousePos3D.x;
			pos.y = mousePos3D.y;
			pos.z = original_z;
			piece.transform.position = pos;
		} else if (dir == JengaBlockScript.Direction.FacingEast || dir == JengaBlockScript.Direction.FacingWest) {
			var original_x = piece.transform.position.x;
			Vector3 mousePos2D = dragPos;
			mousePos2D.x = original_x; // fix the x coordinate when viewing this face
			Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
			Vector3 pos = this.transform.position;
			pos.x = original_x;
			pos.y = mousePos3D.y;
			pos.z = mousePos3D.z;
			piece.transform.position = pos;

			//Debug.Log ("Mouse position X: " + mousePos3D.x + " Y: " + mousePos3D.y + " Z: " + mousePos3D.z);
			//Debug.Log ("Mouse position X: " + mousePos2D.x + " Y: " + mousePos2D.y + " Z: " + mousePos2D.z);
			//Debug.Log ("Move block to X: " + pos.x + " Y: " + pos.y + " Z: " + pos.z);
		}
	
		// East face does not allow for movement left and right, and pulls the thing out toward the camera.
		// South face works with default settings
		// West face does not allow for movement left and right, and pulls the thing out toward the camera.
		// North face works with the default settings

	}
}
