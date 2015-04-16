using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	public static bool musicEnabled = true;
	public static bool soundEffectsEnabled = true;
	AudioSource buttonSound;
	
	void Awake(){
	}
	
	// Use this for initialization
	void Start () {
		buttonSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void enableMusic(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		musicEnabled = true;
	}
	
	public void disableMusic(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		musicEnabled = false;
	}
	
	public void enableSoundEffects(){
		buttonSound.Play();
		soundEffectsEnabled = true;
	}
	
	public void disableSoundEffects(){
		soundEffectsEnabled = false;
	}
}
