using UnityEngine;
using System.Collections;

public class EnemyRotation : MonoBehaviour {

	public float rotSpeed = 90f;
	Transform player;

	// Update is called once per frame
	void Update () {
		if (player == null) {
			// Find player
			GameObject go = GameObject.Find ("Player");

			if (go != null) {
				player = go.transform;
			}
		}

		// Here, player is found or doesn't exist
		if (player == null)
			return; // try next frame

		// by now player is found. turn to face.

		Vector3 dir = player.position - transform.position;
		dir.Normalize ();

		float zAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg - 90;

		Quaternion desiredRotation = Quaternion.Euler (0, 0, zAngle);

		transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRotation, rotSpeed * Time.deltaTime);
	}

}
