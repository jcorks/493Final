using UnityEngine;
using System.Collections;

public class JengaBlockScript : MonoBehaviour {

	public enum Direction {
		FacingNorth,
		FacingEast,
		FacingWest,
		FacingSouth,
		NoDir
	};

	public Direction direction = Direction.NoDir;

	void Awake() {

		float rotation = transform.rotation.eulerAngles.y;
		if ((int)rotation == 270) {
			direction = Direction.FacingNorth;
		} else if ((int)rotation == 0) {
			direction = Direction.FacingEast;
		} else if ((int)rotation == 180) {
			direction = Direction.FacingWest;
		} else if ((int)rotation == 90) {
			direction = Direction.FacingSouth;
		} else {
			direction = Direction.NoDir;
		}
	}

	public bool isMiddle() {
		Ray right = new Ray ();
		Ray left = new Ray ();


		right.direction = (transform.rotation * new Vector3(1, 0f, 0));
		right.origin = transform.position;

		
		left.direction = (transform.rotation * new Vector3(-1, 0f, 0));
		left.origin = transform.position;

		//Debug.DrawRay (left.origin, left.direction * .1f, Color.white, .1f, true);
		//Debug.DrawRay (right.origin, right.direction * .1f, Color.white, .1f, true);

		return (Physics.Raycast (right, 1) && Physics.Raycast (left, 1));
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}
