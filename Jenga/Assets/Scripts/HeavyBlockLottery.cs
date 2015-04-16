using UnityEngine;
using System.Collections;

public class HeavyBlockLottery : MonoBehaviour {
	int number_of_metal_blocks = 20;
	public Material metal_material;
	
	// Use this for initialization
	void Start () {
		int blocks_left = number_of_metal_blocks;
		Transform[] blocks = this.gameObject.GetComponentsInChildren<Transform>();
		while (blocks_left > 0){
			int index = Random.Range(1,blocks.Length);
			blocks[index].gameObject.GetComponent<Renderer>().material = metal_material;
			blocks[index].gameObject.GetComponent<Rigidbody>().mass = 0.075f;
			blocks_left--;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
