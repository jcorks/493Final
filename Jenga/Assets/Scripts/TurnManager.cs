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
		TurnOver,
		GameOver
		
	};

	int numPlayers= 2;
	int curPlayer = 0;

	// The highest piece in the land
	Vector3 topPiecePos;
	// the rotation of the top piece
	Vector3 topPieceRotation;
	// The number of blocks on the top
	int blocks_on_top;


	public float radius = .17f;
	public float degreeDelta = 45f;
	public float radiusDeltaRatio = .5f;
	public Vector3 offset = Vector3.zero;
	public float original_depth;
	Vector3 towerCenter = Vector3.zero;
	
	float pitch =45f,
	yaw   =0f,
	roll  =45f;
	
	
	
	
	Vector3 targetPos;
	Vector3 targetRot;
	
	
	Vector3 selectedOriginalPosition;
	Vector3 selectedOriginalRotation;
	

	bool hasStartedInitialUpdate =false;
	bool hasStartedDragUpdate = false;
	bool hasStartedChooseUpdate = false;
	bool hasStartedDragging = false;
	bool hasStartedReplaceUpdate = false;
	bool hasStartedGameOver = false;


	GameObject gameButton;
	GameObject gameText;
	
	float dragTimer = 0f;
	public Material DragMaterial;
	
	TurnPhase phase = TurnPhase.InitialPhase;
	Vector3 dragPos;


	float turnOverPieceRadius = 1f;
	static int round = 0;


	// Use this for initialization
	void Start () {
		gameButton = GameObject.FindObjectOfType<Button> ().gameObject;
		gameText = GameObject.FindObjectOfType<TextMesh> ().gameObject;
		GameObject tower = GameObject.FindGameObjectWithTag ("Tower");
		if (tower == null) {
			Debug.Log ("WARNING! TurnManager could not find the tower prefab to center itself!");
		} else {
			towerCenter = tower.transform.position;
		}

		GetComponentInChildren<GameOverVisual>().DisableVisual();
	}
	
	// Update is called once per frame
	void Update () {





		//lastPointer = Input.mousePosition;

		
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
		case TurnPhase.ReplacePiece:
			ReplacePieceUpdate();
			break;
		case TurnPhase.TurnOver:
			TurnOverUpdate();
			break;
		case TurnPhase.GameOver:
			GameOverUpdate();
			break;
		}
		
		
	}

	public void GameOver() {
		changePhase (TurnPhase.GameOver);
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
		/*GetComponentInChildren<Text> ().text = "(" + Input.acceleration.x + ", " 
			                                       + Input.acceleration.y + ", "
									               + Input.acceleration.z + ")"; */
	}
	
	
	
	void InitialPhaseUpdate() {
		if (!hasStartedInitialUpdate) {
			Selectable.Freeze ();
			gameText.GetComponent<TextMesh>().text = "Player " + (curPlayer+1) + "'s turn!";
			gameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "";
			var buttonCallback = new Button.ButtonClickedEvent();
			buttonCallback.AddListener(startPicking);
			gameButton.GetComponent<Button>().onClick = buttonCallback;
			hasStartedInitialUpdate = true;
			targetPos = offset + towerCenter + Quaternion.Euler (roll, pitch, yaw) * new Vector3 (0, 0, -radius);
		}

		if (TouchInput.isTouchBegin ()) {
			startPicking ();
		}

	}
	
	
	void ChoosePieceUpdate() {
		if (!hasStartedChooseUpdate) {
			gameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "Pick Piece";
			var buttonCallback = new Button.ButtonClickedEvent();
			buttonCallback.AddListener(FinalizePiece);
			gameButton.GetComponent<Button>().onClick = buttonCallback;
			if (Selectable.GetSelection()) {
				Rigidbody rig = Selectable.GetSelection().GetComponent<Rigidbody>();
				rig.useGravity = true;
			}
			Selectable.Thaw ();
			transform.rotation = Quaternion.Euler (new Vector3 (roll, pitch, yaw));
			hasStartedChooseUpdate = true;

			Selectable.Deselect();
			return;
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
			piece.GetComponent<Rigidbody>().useGravity = false;
			dragTimer = 0f;
		}
		dragTimer += Time.deltaTime;
		
		Vector3 pieceRot = selectedOriginalRotation;
		targetPos = selectedOriginalPosition + 
			Quaternion.Euler (pieceRot.x, pieceRot.y - 90, pieceRot.z) * new Vector3 (0, 0, -.6f*radius);
		transform.LookAt (selectedOriginalPosition);
		
		
		if (dragTimer > 1.4f) {
			var dir = piece.GetComponent<JengaBlockScript> ().direction;
			if (!hasStartedDragging) {
				//dragPos = Camera.main.WorldToScreenPoint(piece.transform.position);
				dragPos = new Vector2(Camera.main.WorldToScreenPoint(piece.transform.position).x,
				                      Camera.main.WorldToScreenPoint(piece.transform.position).y);
				piece.GetComponent<MeshRenderer>().material = DragMaterial;

				if (dir == JengaBlockScript.Direction.FacingSouth || dir == JengaBlockScript.Direction.FacingNorth) {
					original_depth = piece.transform.position.z;
				} else if (dir == JengaBlockScript.Direction.FacingEast || dir == JengaBlockScript.Direction.FacingWest) {
					original_depth = piece.transform.position.x;
				}
				hasStartedDragging = true;
			}


			Debug.DrawLine(Vector3.zero, Camera.main.ScreenToWorldPoint(dragPos));

			if (TouchInput.isTouchBegin()) {
				gameButton.SetActive(false);
			}

			if (TouchInput.tap ()) {
					if (dir == JengaBlockScript.Direction.FacingWest)
						dragPos -= TouchInput.tapDelta();
					else 
						dragPos += TouchInput.tapDelta();
					UserDragPiece ();
				}
			} 
		}



	// Update for when the player has successfully pulled out a piece
	void ReplacePieceUpdate() {
		if (!hasStartedReplaceUpdate) {
			GameObject piece = Selectable.GetSelection();
			//piece.GetComponent<Rigidbody>().useGravity = true;
			transform.rotation = Quaternion.Euler (new Vector3 (roll, pitch, yaw));
			hasStartedReplaceUpdate = true;
			//piece.GetComponent<Rigidbody> ().freezeRotation = false;


			// Get top of tower position
			topPiecePos = Vector3.zero;;
			float top = -999;
			GameObject topPiece = null;
			GameObject[] pieces = GameObject.FindGameObjectsWithTag("JengaBlock");
			foreach(GameObject o in pieces) {
				if (o.transform.position.y > top) {
					topPiece = o;
					top = o.transform.position.y;
				}
			}
			topPiecePos = topPiece.transform.position;
			topPieceRotation = topPiece.transform.rotation.eulerAngles;
			
			// Get the number of pieces on highest layer
			blocks_on_top = 0;
			foreach(GameObject o in pieces) {
				// If the tower is tilted, the adjacent blocks might not have
				// exactly the same y value
				var upper_bound = topPiecePos.y + 0.01;
				var lower_bound = topPiecePos.y - 0.01;
				if (o.transform.position.y > lower_bound && o.transform.position.y < upper_bound) {
					++blocks_on_top;
				}
			}
			Debug.Log("There " + (blocks_on_top > 1 ? " are " : " is ") + blocks_on_top +
					 (blocks_on_top > 1 ? " blocks on the top layer" : " block on the top layer "));

			Vector3 pieceRot = selectedOriginalRotation;
			targetPos = topPiece.transform.position +  new Vector3 (-.6f*radius, .3f*radius, -.6f*radius);
			transform.LookAt (topPiece.transform.position);
			selectedOriginalPosition = topPiece.transform.position;



		}

		transform.LookAt (selectedOriginalPosition);

		if (TouchInput.tap ()) {
			var dir = Selectable.GetSelection().GetComponent<JengaBlockScript> ().direction;
			if (dir == JengaBlockScript.Direction.FacingWest)
				dragPos -= TouchInput.tapDelta();
			else 
				dragPos += TouchInput.tapDelta();
			
			
			UserReplacePiece ();
		} 
	}

	void TurnOverUpdate() {
		round++;
		curPlayer++;
		if (curPlayer == numPlayers) {
			curPlayer = 0;
		}

	}


	void GameOverUpdate() {
		if (!hasStartedGameOver) {

			targetPos = new Vector3(-.18f*radius, .8f*radius, -.18f*radius) +
				GameObject.FindGameObjectWithTag ("Tower").transform.position;
			var buttonCallback = new Button.ButtonClickedEvent();
			buttonCallback.AddListener(endGame);
			gameButton.GetComponent<Button>().onClick = buttonCallback;
			gameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = "OK";
			GetComponentInChildren<GameOverVisual>().EnableVisual();
		}

		transform.LookAt (GameObject.FindGameObjectWithTag ("Tower").transform.position);

	}

	// Takes the currently selected piece and prepares it for movement.
	public void FinalizePiece() {
		changePhase(TurnPhase.DragPiece);
		//Destroy(Selectable.GetSelection ());
	}
	
	public void UndoPiece() {
		changePhase ( TurnPhase.ChoosePiece);
	}

	public void startPicking() {
		gameText.GetComponent<TextMesh> ().text = "";
		changePhase (TurnPhase.ChoosePiece);
	}
	
	void changePhase(TurnPhase p) {
		hasStartedDragUpdate = false;
		hasStartedChooseUpdate = false;
		hasStartedDragging = false;
		hasStartedReplaceUpdate = false;
		hasStartedGameOver = false;
		gameButton.SetActive (true);

		phase = p;
	}

	void endGame() {
		Application.LoadLevel ("MainMenu");
	}












	////// User updates
	bool piece_has_teleported = false;
	// put logic here for re placing the piece.
	// need to initially on first call place the block properly
	float fixed_height;
	void UserReplacePiece() {
		GameObject piece = Selectable.GetSelection();
		var selected_piece_rotation = piece.transform.rotation.eulerAngles;
		var selected_piece_rotation_upper_limit = selected_piece_rotation + new Vector3(0f, 0.01f, 0f);
		var selected_piece_rotation_lower_limit = selected_piece_rotation - new Vector3(0f, 0.01f, 0f);

		Vector3 new_position = topPiecePos;


		if (!piece_has_teleported) {
			new_position.x += 0.05f;
			new_position.z += 0.05f;
			if (blocks_on_top == 3) { // start new layer
				new_position.y += 0.017f;
				fixed_height = new_position.y;

				//if (topPieceRotation < selected_piece_rotation_upper_limit && )

			} else { // add to current highest layer
				new_position.y += 0.003f;
			}
			piece.transform.position = new_position;
			// Keep it from floating away if you're using a mouse
			piece.GetComponent<Rigidbody> ().velocity = Vector3.zero;

			dragPos = Camera.main.WorldToScreenPoint(piece.transform.position);
			piece_has_teleported = true;
		}

		// Get the camera's position to normalize piece movement
		//Vector2 dragPos = new Vector2(Camera.main.WorldToScreenPoint(piece.transform.position).x,
		  //                    Camera.main.WorldToScreenPoint(piece.transform.position).z);

		Vector3 drag_position = Camera.main.ScreenToWorldPoint(dragPos);
		drag_position.y = fixed_height;

		piece.transform.position = drag_position;
		Debug.Log("UserReplacePiece()");

//		Vector3 start_pos;




/*		if(Input.GetKey(KeyCode.MouseDown)){
			piece.transform.position = dragPos2;		
		}*/



		/*var dir = piece.GetComponent<JengaBlockScript> ().direction;
		if (dir == JengaBlockScript.Direction.FacingWest) {
			dragPos -= TouchInput.tapDelta();	
		} else {
			dragPos += TouchInput.tapDelta();
		}

		if (dir == JengaBlockScript.Direction.FacingSouth || dir == JengaBlockScript.Direction.FacingNorth) {
			//var original_z = piece.transform.position.z;

			dragPos.z = original_depth; // fix the z coordinate when viewing this face
			Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (dragPos);
			Vector3 pos = this.transform.position;
			pos.x = mousePos3D.x;
			pos.y = piece.transform.position.y;
			pos.z = original_depth;
			piece.transform.position = pos;
		} else if (dir == JengaBlockScript.Direction.FacingEast || dir == JengaBlockScript.Direction.FacingWest) {
			var left_right = dragPos.x;
			var back_forth = dragPos.z;
			Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (new Vector3(left_right, back_forth, original_depth));
			Vector3 pos = -(this.transform.position);
			pos.x = original_depth;
			pos.y = piece.transform.position.y;
			pos.z = mousePos3D.z;
			piece.transform.position = pos;
		}*/



		// Have the user place it wherever they want
		
		//Vector3 mousePos2D = Input.mousePosition;
		
		/*mousePos2D.y = piece.transform.position.y;
		Vector3 mousePos2D = dragPos;
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);*/




		// ********** DON'T DELETE **********
		// changePhase(TurnPhase.TurnOver);
	}
	
	
	// Put logic here for dragging the piece
	void UserDragPiece() {
		// Get the z coordinate of the piece you wanna drag
		GameObject piece = Selectable.GetSelection();
		piece.GetComponent<Rigidbody> ().freezeRotation = true;
		var dir = piece.GetComponent<JengaBlockScript> ().direction;
		//Debug.Log ("this piece's rotation: " + rotation);

		Vector3 mousePos2D = dragPos;

		if (dir == JengaBlockScript.Direction.FacingSouth || dir == JengaBlockScript.Direction.FacingNorth) {
			//var original_z = piece.transform.position.z;

			mousePos2D.z = original_depth; // fix the z coordinate when viewing this face
			Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
			Vector3 pos = this.transform.position;
			pos.x = mousePos3D.x;
			pos.y = mousePos3D.y;
			pos.z = original_depth;
			piece.transform.position = pos;
		} else if (dir == JengaBlockScript.Direction.FacingEast || dir == JengaBlockScript.Direction.FacingWest) {
			// Mouse never moves in the z direction, even though the camera changes angle
			//var depth = piece.transform.position.x;
			//Vector3 mousePos2D = -Input.mousePosition;
			var left_right = mousePos2D.x;
			var up_down = mousePos2D.y;


			Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (new Vector3(left_right, up_down, original_depth));
			Vector3 pos = -(this.transform.position);
			pos.x = original_depth;
			pos.y = mousePos3D.y;
			pos.z = mousePos3D.z;
			piece.transform.position = pos;

			// Set pos.z to mouse position x?


			//Debug.Log ("Mouse 3D position X: " + mousePos3D.x + " Y: " + mousePos3D.y + " Z: " + mousePos3D.z);
			//Debug.Log ("Mouse 2D position X: " + mousePos2D.x + " Y: " + mousePos2D.y + " Z: " + mousePos2D.z);
			//Debug.Log ("Move block to X: " + pos.x + " Y: " + pos.y + " Z: " + pos.z);
		}


		// Detect whether or not the piece was dragged out
		Rigidbody pieceRig = piece.GetComponent<Rigidbody> (); 

		RaycastHit hit;
		if (!pieceRig.SweepTest(new Vector3(0, 1, 0), out hit,  1) ||
		    !pieceRig.SweepTest(new Vector3(0, -1, 0), out hit, 1)) {

			changePhase(TurnPhase.ReplacePiece);
		}
		
		// East face does not allow for movement left and right, and pulls the thing out toward the camera.
		// South face works with default settings
		// West face does not allow for movement left and right, and pulls the thing out toward the camera.
		// North face works with the default settings
		
		//piece.GetComponent<JengaBlockScript>().direction
	}
}