using UnityEngine;
using System.Collections;

public class MoveForwardArrow : MonoBehaviour {
	
	public float maxSpeed = 10f;

	void Start() {
		GameObject player = GameObject.Find ("Player");
		maxSpeed = player.GetComponent <playerShooting> ().getDrawStrength();
		player.GetComponent <playerShooting> ().resetDrawStrength();
	}

	// Update is called once per frame
	void Update () {

		Vector3 pos = transform.position;

		Vector3 velocity = new Vector3 (0, maxSpeed * Time.deltaTime, 0);
		
		pos += transform.rotation * velocity;
		
		transform.position = pos;
	}
}
