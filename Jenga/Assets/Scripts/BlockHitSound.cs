using UnityEngine;
using System.Collections;

public class BlockHitSound : MonoBehaviour {
	float threshold = 0.1f;
	AudioSource soundEffect;
	float timer = 0;
	
	// Use this for initialization
	void Start () {
		soundEffect = this.gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
	}
	
	void OnCollisionEnter(Collision other){
		if (timer < .5f)
			return;
		if (other.relativeVelocity.magnitude > threshold){
			if (!MusicController.soundEffectsEnabled)
				return;
			soundEffect.Play();
			soundEffect.volume = other.relativeVelocity.magnitude/1.5f;
			soundEffect.pitch = Random.Range(0.75f,1.1f);
		}
	}
}
