using UnityEngine;
using System.Collections;


/* a class the represents an object that can be selected uniquely out of a group of all seletables. */
public class Selectable : MonoBehaviour {
	
	static GameObject selectedRef = null;
	Material actualMat = null;
	public Material selectedMat;
	public static bool enableSelection = true;
	MeshRenderer meshRenderer = null;


	// Selects the object and deselects anything else currently selected
	public void Select() {
		if (!enableSelection)
			return;
		if (selectedRef) {
			selectedRef.GetComponent<Selectable>().meshRenderer.material = 
		    selectedRef.GetComponent<Selectable>().actualMat;
		}
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
		}
		selectedRef = null;
	}




	void Awake() {
		meshRenderer = GetComponent<MeshRenderer> ();

	}

	void Start() {
		if (!meshRenderer)
			Destroy (this.gameObject);

		actualMat = meshRenderer.material;
	}




}
