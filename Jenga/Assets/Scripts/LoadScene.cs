using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {
	AudioSource buttonSound;
	
	// Use this for initialization
	void Start () {
		buttonSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void loadOptions(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("OptionsMenu");
	}
	
	public void loadMainMenu(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("MainMenu");
	}
	
	public void loadModeSelect(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("ModeSelect");
	}
	
	public void loadChallenges(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("ChallengeSelect");
	}
	
	public void loadGame(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("jc_scene");
	}
	public void loadHeavyBlockReady(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("HeavyBlockReady");
	}
	
	public void loadHeavyBlock(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("HeavyBlockScene");
	}
	
	public void loadScoreAttackReady(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("ScoreAttackReady");
	}
	
	public void loadScoreAttack(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("ScoreAttack");
	}
	
	public void loadHighscoreList(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("HighscoresList");
	}
	
	public void loadQuit(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.Quit();
	}
	
	public void loadClassicPlayerSelect(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("ClassicPlayerSelect");
	}
	
	public void loadHeavyBlockPlayerSelect(){
		if (MusicController.soundEffectsEnabled)
			buttonSound.Play();
		Application.LoadLevel("HeavyBlockPlayerSelect");
	}
		
}
