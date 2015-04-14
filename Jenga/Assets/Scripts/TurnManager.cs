using UnityEngine;
using System.Collections;

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

	TurnPhase phase = TurnPhase.ChoosePiece;

	// Use this for initialization
	void Start () {
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
		hasStartedDragUpdate = false;
		phase = TurnPhase.ChoosePiece;
	}
	
	
	void ChoosePieceUpdate() {
		if (TouchInput.swipeLeft())
			pitch -= degreeDelta;
		
		if (TouchInput.swipeRight ()) 
			pitch += degreeDelta;
		
		
		if (TouchInput.swipeUp ()) {
			if (roll == 270) return;
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
		}
		Vector3 pieceRot = selectedOriginalRotation;
		targetPos = selectedOriginalPosition + 
			Quaternion.Euler (pieceRot.x, pieceRot.y - 90, pieceRot.z) * new Vector3 (0, 0, -.4f*radius);
		transform.LookAt (selectedOriginalPosition);


		UserDragPiece ();

	}

	// Takes the currently selected piece and prepares it for movement.
	public void FinalizePiece() {
		phase = TurnPhase.DragPiece;
		//Destroy(Selectable.GetSelection ());
	}


	// Put logic here for dragging the piece
	void UserDragPiece() {

	}
}
