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

	int numPlayers;
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
	bool hasStartedTurnOver = false;
	bool hasStartedGameOver = false;


	GameObject gameButton;
	GameObject gameText;
	GameObject DragHelperSprite;
	GameObject ReplaceHelperSprite;
	
	float dragTimer = 0f;
	float replaceTimer = 0f;
	float stabilizeTimer = 0f;
	public Material DragMaterial;
	
	TurnPhase phase = TurnPhase.InitialPhase;
	Vector3 dragPos;


	float turnOverPieceRadius = 1f;
	public static int round = 0;



	bool was_middle;










	// // // Built-in UNITY FUNCTIONS // // // //

	// Use this for initialization
	void Start () {
		numPlayers = PlayerSelect.num_players;
		gameButton = GameObject.Find("Button");
		gameText = GameObject.FindObjectOfType<TextMesh> ().gameObject;
		GameObject tower = GameObject.FindGameObjectWithTag ("Tower");
		if (tower == null) {
			Debug.Log ("WARNING! TurnManager could not find the tower prefab to center itself!");
		} else {
			towerCenter = tower.transform.position;
		}
		DragHelperSprite = GameObject.Find ("DragHelper");
		ReplaceHelperSprite = GameObject.Find ("ReplaceHelper");
		round = 0;

		GetComponentInChildren<GameOverVisual>().DisableVisual();



		// Sets the Criterion to disable selection of bottom row
		float best = 999f;
		GameObject[] blocks = GameObject.FindGameObjectsWithTag ("JengaBlock");
		foreach (GameObject o in blocks) {
			if (o.transform.position.y < best)
				best = o.transform.position.y;
		}
		Selectable.CriterionMinY (best + .001f);
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
















	// // PHASE UPDATES // // //
	// Phase updates are in sequential order except for GameOver, which can hapen at any time.
	
	// Phase for when the player is getting ready
	void InitialPhaseUpdate() {
		if (!hasStartedInitialUpdate) {
			Selectable.Freeze ();
			gameText.GetComponent<TextMesh>().text = "Player " + (curPlayer+1) + "'s turn!";
			gameButton.SetActive(false);
			hasStartedInitialUpdate = true;
			targetPos = offset + towerCenter + Quaternion.Euler (roll, pitch, yaw) * new Vector3 (0, 0, -radius);
		}

		if (TouchInput.isTouchBegin ()) {
			gameText.GetComponent<TextMesh> ().text = "";
			changePhase (TurnPhase.ChoosePiece);
		}

	}
	
	// Update for when the player is choosing their piece.
	// The camera can be controlled with swiping.
	void ChoosePieceUpdate() {
		if (!hasStartedChooseUpdate) {
			gameButton.SetActive(true);
			updateButton (ButtonCallback_FinalizePiece, "Pick Piece!");


			Selectable.Thaw ();
			ResetSelected();
			transform.rotation = Quaternion.Euler (new Vector3 (roll, pitch, yaw));
			hasStartedChooseUpdate = true;


			// disable selection of top 2 rows
			updateTopInfo();
			Selectable.CriterionMaxY(topPiecePos.y - .035f);


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
	
	// Update for when the player is dragging their piece from the tower
	void DragPieceUpdate() {
		GameObject piece = Selectable.GetSelection();
		if (!hasStartedDragUpdate) {
			selectedOriginalPosition = piece.transform.position;
			selectedOriginalRotation = piece.transform.rotation.eulerAngles;

			hasStartedDragUpdate = true;
			Selectable.Freeze ();
			updateButton(ButtonCallback_UndoPiece, "Back");

			piece.GetComponent<Rigidbody>().useGravity = false;
			piece.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			DragHelperSprite.SetActive(true);
			dragTimer = 0f;
		}
		dragTimer += Time.deltaTime;
		
		Vector3 pieceRot = selectedOriginalRotation;
		var dir = piece.GetComponent<JengaBlockScript> ().direction;
		if (was_middle && dir == JengaBlockScript.Direction.FacingEast){
			targetPos = selectedOriginalPosition + 
				Quaternion.Euler (pieceRot.x, pieceRot.y + 90, pieceRot.z) * new Vector3 (0, 0, -.6f * radius);
		}
		else
			targetPos = selectedOriginalPosition + 
				Quaternion.Euler (pieceRot.x, pieceRot.y - 90, pieceRot.z) * new Vector3 (0, 0, -.6f * radius);
		transform.LookAt (selectedOriginalPosition);

		updateDragHelper ();
		
		if (dragTimer > 1.4f) {
			//var dir = piece.GetComponent<JengaBlockScript> ().direction;
			if (!hasStartedDragging) {
				//dragPos = Camera.main.WorldToScreenPoint(piece.transform.position);



				dragPos = new Vector2(Camera.main.WorldToScreenPoint(piece.transform.position).x,
				                      Camera.main.WorldToScreenPoint(piece.transform.position).y);
				piece.GetComponent<MeshRenderer>().material = DragMaterial;
				// Drag middle piece works for East and West faces

				if (dir == JengaBlockScript.Direction.FacingSouth || dir == JengaBlockScript.Direction.FacingNorth) {
					original_depth = piece.transform.position.z;
				} else if (dir == JengaBlockScript.Direction.FacingEast || dir == JengaBlockScript.Direction.FacingWest) {
					original_depth = piece.transform.position.x;
				}
				hasStartedDragging = true;
			}




			if (TouchInput.tap ()) {
				if (TouchInput.tapDelta() != Vector3.zero)
						gameButton.SetActive(false);
				if (was_middle && dir == JengaBlockScript.Direction.FacingEast){
					Debug.Log("Buggy middle block");
					dragPos -= 10*TouchInput.tapDelta();
				}
				else if (dir == JengaBlockScript.Direction.FacingEast) {
					Debug.Log("Facing east");
					dragPos += 4*TouchInput.tapDelta();
				}
				 else if (dir == JengaBlockScript.Direction.FacingWest) {
					Debug.Log("Facing west");
					dragPos -= 4*TouchInput.tapDelta();
				} else if (dir == JengaBlockScript.Direction.FacingNorth){
					Debug.Log ("Facing north");
					dragPos += TouchInput.tapDelta();
				} else if (dir == JengaBlockScript.Direction.FacingSouth){
					Debug.Log ("Facing south");
					dragPos += TouchInput.tapDelta();
				}
					UserDragPiece ();
				}
			} 
		}



	// Update for when the player has successfully pulled out a piece
	void ReplacePieceUpdate() {
		if (!hasStartedReplaceUpdate) {
			GameObject piece = Selectable.GetSelection();
			transform.rotation = Quaternion.Euler (new Vector3 (roll, pitch, yaw));
			hasStartedReplaceUpdate = true;


			// Get top of tower position
			updateTopInfo();

			Debug.Log("There " + (blocks_on_top > 1 ? " are " : " is ") + blocks_on_top +
					 (blocks_on_top > 1 ? " blocks on the top layer" : " block on the top layer "));




			Vector3 pieceRot = selectedOriginalRotation;
			targetPos = topPiecePos +  new Vector3 (-.5f*radius, .8f*radius, -.5f*radius);
			transform.LookAt (topPiecePos);
			selectedOriginalPosition = topPiecePos	;

			updateButton(ButtonCallback_PlacedPiece, "Place!");

			replaceTimer = 0f;


		}
		transform.LookAt (selectedOriginalPosition);
		replaceTimer += Time.deltaTime;
		if (replaceTimer < 1f)
			return;
		ReplaceHelperSprite.SetActive (true);



		updateReplaceHelper ();
		if (TouchInput.tap ()) {
			dragPos += TouchInput.tapDelta();
				
			UserReplacePiece ();
		} 
	}

	//Update for when the turn is coming to an end
	// Delays starting for .5f seconds to get the stabilization checks a head start.
	// Then, waits until the tower stops moving. Once it does stabilize, goes to the next turn
	void TurnOverUpdate() {

		if (!hasStartedTurnOver) {
			targetPos = new Vector3(-.18f*radius, .8f*radius, -.18f*radius) +
				GameObject.FindGameObjectWithTag ("Tower").transform.position;

			Selectable.Thaw ();
			ResetSelected ();
			Selectable.Freeze ();
			hasStartedTurnOver = true;
			stabilizeTimer = 0f;
			gameButton.SetActive(false);
		}
		transform.LookAt (GameObject.FindGameObjectWithTag ("Tower").transform.position);

		stabilizeTimer += Time.deltaTime;
		if (stabilizeTimer < .5f)
			return;



		// wait for stabliztion;
		if (!isStablized ())
			return;

		round++;
		curPlayer++;
		if (curPlayer == numPlayers) {
			curPlayer = 0;
		}
		Debug.Log ("TURNS OVER NOW");
		was_middle = false;
		changePhase (TurnPhase.InitialPhase);
	}

	// Displays JENGA! visual and waits for player before returning to title screen.
	void GameOverUpdate() {
		if (!hasStartedGameOver) {

			targetPos = new Vector3(-.4f*radius, .8f*radius, -.4f*radius) +
				GameObject.FindGameObjectWithTag ("Tower").transform.position;
			updateButton(ButtonCallback_EndGame, "OK");
			GetComponentInChildren<GameOverVisual>().EnableVisual();
		}
		transform.LookAt (GameObject.FindGameObjectWithTag ("Tower").transform.position);
	}










	 // // CALLBACKS // // //

	// Takes the currently selected piece and prepares it for movement.
	public void ButtonCallback_FinalizePiece() {
		was_middle = Selectable.GetSelection ().gameObject.GetComponent<JengaBlockScript> ().isMiddle ();
		changePhase(TurnPhase.DragPiece);
		//Destroy(Selectable.GetSelection ());
	}
	
	public void ButtonCallback_UndoPiece() {
		changePhase ( TurnPhase.ChoosePiece);
	}


	public void ButtonCallback_PlacedPiece() {
		changePhase (TurnPhase.TurnOver);
	}

	void ButtonCallback_EndGame() {
		Application.LoadLevel ("MainMenu");
	}
	













	// UTILITY FUNCTIONS

	// Redraws the update, 2-arrow visual
	void updateDragHelper() {
		DragHelperSprite.transform.position = Selectable.GetSelection().transform.position + 
			(DragHelperSprite.transform.rotation * new Vector3(0f, 0f, -.03f));
		DragHelperSprite.transform.localScale = new Vector3 (
			.01f*(1f + .3f *Mathf.Sin (3*dragTimer)), 
			.01f*(1f + .3f *Mathf.Cos (3*dragTimer)), 1f);
	}

	// Redraws the update, 4-arrow visual
	void updateReplaceHelper() {
		ReplaceHelperSprite.transform.position = Selectable.GetSelection().transform.position + 
			(new Vector3(0f, .01f, 0f));
		//ReplaceHelperSprite.transform.rotation = 
		ReplaceHelperSprite.transform.localScale = new Vector3 (
			.02f*(1f + .3f *Mathf.Sin (3*replaceTimer)), 
			.02f*(1f + .3f *Mathf.Cos (3*replaceTimer)), 1f);
	}

	// returns whether or not the tower is stabilized. When stable,
	// the tower physics is sleeping and will not move. 
	public bool isStablized() {
		GameObject[] blocks = GameObject.FindGameObjectsWithTag ("JengaBlock");
		bool sleep = true;
		foreach (GameObject o in blocks) {
			if (!o.GetComponent<Rigidbody>().IsSleeping()) {
				sleep = false;
				break;
			}
		}
		return sleep;

	}

	// Resets the selected piece to be back to normal game mode,
	// where physics is enabled and it is deselected
	public void ResetSelected() {
		if (Selectable.GetSelection()) {
			Rigidbody rig = Selectable.GetSelection().GetComponent<Rigidbody>();
			rig.useGravity = true;
			rig.freezeRotation = false;
			rig.constraints = RigidbodyConstraints.None;
			Debug.Log ("Reset gravity");
		}

		Selectable.Deselect();
	}

	// Changes the turn's phase.
	void changePhase(TurnPhase p) {
		hasStartedDragUpdate = false;
		hasStartedChooseUpdate = false;
		hasStartedDragging = false;
		hasStartedReplaceUpdate = false;
		hasStartedGameOver = false;
		hasStartedTurnOver = false;
		hasStartedInitialUpdate = false;
		piece_has_teleported = false;

		DragHelperSprite.SetActive (false);
		ReplaceHelperSprite.SetActive (false);
		gameButton.SetActive (true);

		phase = p;
	}

	// Sets the game button event function and displayed text.

	void updateButton(UnityEngine.Events.UnityAction cb, string text) {
		var buttonCallback = new Button.ButtonClickedEvent();
		buttonCallback.AddListener(cb);
		gameButton.GetComponent<Button>().onClick = buttonCallback;
		gameButton.GetComponent<Button>().GetComponentInChildren<Text>().text = text;

	}
	// Public function to end the game.
	public void GameOver() {
		changePhase (TurnPhase.GameOver);
	}


	// sets the 
	// topPiecePos 			(which contains the position of the highest piece)
	// topPieceROtation 	(the rotation of the aforementioned piece)
	// blocks_on_top		(the number of blcoks currently on the top of the tower);
	void updateTopInfo() {
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
	}
	
	
	
	
	
	
	
	
	
	////// User updates
	/// Here is the logic for functiosn that require mouse-to-world dragging of objects
	
	// put logic here for re placing the piece.
	// need to initially on first call place the block properly
	float fixed_height;
	float placement_rotation = 90f;
	bool facing_same_direction = false;
	bool piece_has_teleported = false;
	void UserReplacePiece() {
		
		GameObject piece = Selectable.GetSelection();
		Vector3 new_position = topPiecePos;


		Vector3 temp_rotation = piece.transform.rotation.eulerAngles;
		temp_rotation.y = placement_rotation;
		piece.transform.rotation = Quaternion.Euler(temp_rotation);

		if (!piece_has_teleported) {
			if ((round % 3) == 0) {
				if (placement_rotation == 90f) {
					placement_rotation = 0f;
				} else {
					placement_rotation = 90f;
				}
			}
			new_position.x = 0.05408994f;
			new_position.z = 0.1280344f;
			if (blocks_on_top == 3) { // start new layer
				new_position.y += 0.02f;
				fixed_height = new_position.y;
			} else { // add to current highest layer
				new_position.y += 0.009f;
			}
			// Keep it from floating away if you're using a mouse
			piece.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			dragPos = Camera.main.WorldToScreenPoint(piece.transform.position);
			piece_has_teleported = true;
		}
		piece.transform.position = new_position;
		Vector3 drag_position = Camera.main.ScreenToWorldPoint(dragPos);
		drag_position.y = fixed_height;
		piece.transform.position = drag_position;
		Debug.Log("UserReplacePiece()");
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
			Debug.Log ("facing: " + dir);
		} else if (dir == JengaBlockScript.Direction.FacingEast || dir == JengaBlockScript.Direction.FacingWest) {
			// Mouse never moves in the z direction, even though the camera changes angle
			//var depth = piece.transform.position.x;
			//Vector3 mousePos2D = -Input.mousePosition;
			var left_right = mousePos2D.x;
			var up_down = mousePos2D.y;


			Vector3 mousePos3D = Camera.main.ScreenToWorldPoint (new Vector3(left_right, up_down, original_depth));
			Vector3 pos = -(this.transform.position);
			pos.x = original_depth;
			/*if (piece.GetComponent<JengaBlockScript>().isMiddle ()) {
				mousePos2D.z = original_depth; // fix the z coordinate when viewing this face
				Vector3 mousePos3D_ = Camera.main.ScreenToWorldPoint (mousePos2D);
				pos = this.transform.position;
				pos.x = mousePos3D_.x;
				pos.y = mousePos3D_.y;
				pos.z = original_depth;
				piece.transform.position = pos;
			} else {*/
				pos.y = mousePos3D.y;
				pos.z = mousePos3D.z;
				piece.transform.position = pos;
			//}
			Debug.Log ("facing: " + dir);

			// Thinks middle pieces that're facing south are actually facing east/west


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