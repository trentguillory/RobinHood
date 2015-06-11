using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {
	
	private Vector3 cameraTarget;
	
	private Transform target;
	
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").transform;
		
	}
	
	// Update is called once per frame
	void Update () {
		cameraTarget = new Vector3 (target.position.x, target.position.y, target.position.z - 10);
		transform.position = cameraTarget;
	}
}