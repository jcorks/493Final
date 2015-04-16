using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	public static bool musicEnabled = true;
	public static bool soundEffectsEnabled = true;
	
	
	void Awake(){
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void enableMusic(){
		musicEnabled = true;
	}
	
	public void disableMusic(){
		musicEnabled = false;
	}
	
	public void enableSoundEffects(){
		soundEffectsEnabled = true;
	}
	
	public void disableSoundEffects(){
		soundEffectsEnabled = false;
	}
}
