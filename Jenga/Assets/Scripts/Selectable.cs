using UnityEngine;
using System.Collections;


/* a class the represents an object that can be selected uniquely out of a group of all seletables. */
public class Selectable : MonoBehaviour {
	
	static GameObject selectedRef = null;
	Material actualMat = null;
	public Material selectedMat;
	public Material transparentMat;
	public static bool enableSelection = true;
	MeshRenderer meshRenderer = null;
	static float maxY = 999f;
	static float minY = -999f;
	
	// Selects the object and deselects anything else currently selected
	public void Select() {
		if (!enableSelection || transform.position.y > maxY || transform.position.y < minY)
			return;
		if (selectedRef) {
			selectedRef.GetComponent<Selectable>().meshRenderer.material = 
		    selectedRef.GetComponent<Selectable>().actualMat;
		    restoreOpaquenessToAll();
		}
		if (gameObject.GetComponent<JengaBlockScript>().isMiddle())
			makeOtherBlocksTransparent();
		meshRenderer.material = selectedMat;
		selectedRef = gameObject;
	}

	// Returns whether or not this object is current selected.
	public bool IsSelected() {
		return selectedRef == gameObject;
	}


	public static GameObject GetSelection() {
		return selectedRef;
	}


	public static void Freeze() {
		enableSelection = false;
	}

	public static void Thaw() {
		enableSelection = true;
	}

	public static void Deselect() {
		if (!enableSelection)
			return;
		if (selectedRef) {
			selectedRef.GetComponent<Selectable>().meshRenderer.material = 
			selectedRef.GetComponent<Selectable>().actualMat;
			restoreOpaquenessToAll();
		}
		selectedRef = null;
	}

	// TO be selectable this object's y position must be below this value
	public static void CriterionMaxY(float f) {
		maxY = f;
	}

	// TO be selectable, this object's y position must be above this value
	public static void CriterionMinY(float f) {
		minY = f;
	}




	void Awake() {
		meshRenderer = GetComponent<MeshRenderer> ();

	}

	void Start() {
		if (!meshRenderer)
			Destroy (this.gameObject);
		StartCoroutine(delayedSetup());
	}
	
	IEnumerator delayedSetup(){
		float i = 0;
		while (i < 0.1f){
			i += Time.deltaTime;
			yield return 0;
		}
		actualMat = meshRenderer.material;
	}

	void makeOtherBlocksTransparent(){
		GameObject tower = this.gameObject.transform.parent.gameObject;
		foreach (Transform block_transform in tower.GetComponentsInChildren<Transform>()){
			if (block_transform.gameObject != this.gameObject && block_transform.gameObject != tower){
				block_transform.gameObject.GetComponent<Renderer>().material = transparentMat;
			}
		}
	}
	
	static void restoreOpaquenessToAll(){
		GameObject tower = GameObject.Find("Tower");
		foreach (Transform block_transform in tower.GetComponentsInChildren<Transform>()){
			if (block_transform.gameObject != tower){
				block_transform.gameObject.GetComponent<Renderer>().material = 
				block_transform.gameObject.GetComponent<Selectable>().actualMat;
			}
		}
	}

}
