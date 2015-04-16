using UnityEngine;
using System.Collections;

public class BlockHitSound : MonoBehaviour {
	float threshold = 0.5f;
	AudioSource soundEffect;
	
	// Use this for initialization
	void Start () {
		soundEffect = this.gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision other){
		if (other.relativeVelocity.magnitude > threshold){
			soundEffect.Play();
			soundEffect.volume = other.relativeVelocity.magnitude/4f;
			soundEffect.pitch = Random.Range(0.9f,1.1f);
		}
	}
}
