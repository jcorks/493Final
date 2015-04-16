using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {
	AudioSource music;
	
	// Use this for initialization
	void Start () {
		GameObject[] musics = GameObject.FindGameObjectsWithTag("Music");
		if (musics.Length > 1)
			Destroy(musics[1]);
		music = this.gameObject.GetComponent<AudioSource>();
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (music.isPlaying && !MusicController.musicEnabled)
			music.Stop();
		else if (!music.isPlaying && MusicController.musicEnabled)
			music.Play();
	}
}
