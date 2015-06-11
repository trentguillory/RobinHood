using UnityEngine;
using System.Collections;

public class playerShooting : MonoBehaviour {

	public Vector3 bulletOffset = new Vector3 (0, 0.5f, 0);
	public GameObject arrowPrefab;
	public float reloadDelay = 1f;
	float reloadTimer = 0;
	
	public float drawStrength = 0f;
	bool stringIsReleased = false;
	//float drawTimer = 0;

	//Animator anim2;

	void Start () {
		//anim2 = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		reloadTimer -= Time.deltaTime;

		if (Input.GetButton ("Fire1") && reloadTimer <= 0) {
			drawStrength += Time.deltaTime * 7;
			//anim2.SetFloat ("isDrawing", 1f);
			stringIsReleased = false;
		}

		if ( stringIsReleased == true && drawStrength > 0.01f) {

			reloadTimer = reloadDelay;
				
			Vector3 offset = transform.rotation * bulletOffset;
				
			Instantiate (arrowPrefab, transform.position + offset, transform.rotation);

			//drawStrength = 0;
		}
		//anim2.SetFloat ("isDrawing", 0f);
		stringIsReleased = true;

	}

	public float getDrawStrength() {
		return drawStrength;
	}
	public void resetDrawStrength() {
		drawStrength = 0;
	}
}
