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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
