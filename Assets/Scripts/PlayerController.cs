using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	// Handling variables
	public float health = 1;
	public float rotSpeed = 450;
	public float walkSpeed = 5;
	public float runSpeed = 8;
	public float boundY = 3.5f;
	public float boundX = 5;

	Animator anim;
	Rigidbody2D rigbod;

	// System
	private Quaternion targetRotation;
	private Vector3 currentVelocityMod;
	private Camera cam;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		anim = GetComponent<Animator> ();
		rigbod = GetComponent<Rigidbody2D> ();
		anim.SetFloat ("health", (float)health);
	}

	void OnTriggerEnter2D() {
		health--;
		if (health < 1) {
			if (health <= 0) {
				anim.SetFloat ("health", (float)health);
				StartCoroutine(pauseForDeathAnimation());
			}
		}
	}

	void FixedUpdate() {
		// disables player from bouncing away from objects
		rigbod.velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if (health >= 1) {
			controlMouse ();

			if (Input.GetButton ("Fire1")) {
				anim.SetFloat ("isDrawing", 1);
			} else {
				anim.SetFloat ("isDrawing", 0);
			}
		}
	}

	void controlMouse() {
		// ROTATION
		var mouse = Input.mousePosition;
		var screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
		var offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
		var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, 0, angle - 90);

		// MOVEMENT

		if ((Input.GetAxis ("Horizontal")) != 0 || (Input.GetAxis ("Vertical")) != 0) {
			anim.SetFloat ("speed", 1);
		} else {
			anim.SetFloat ("speed", 0);
		}

		// was transform.position
		Vector3 pos = rigbod.position;

		//this method of compensation for diagonal movement is iffy
		if (Input.GetKey ("w") && Input.GetKey ("d") || Input.GetKey ("w") && Input.GetKey ("a") || Input.GetKey ("s") && Input.GetKey ("d") || Input.GetKey ("s") && Input.GetKey ("a")) {
			pos.y += Input.GetAxis ("Vertical") * walkSpeed * .71f * Time.deltaTime;
			pos.x += Input.GetAxis ("Horizontal") * walkSpeed * .71f * Time.deltaTime;
		} else {
			pos.y += Input.GetAxis ("Vertical") * walkSpeed * Time.deltaTime;
			pos.x += Input.GetAxis ("Horizontal") * walkSpeed * Time.deltaTime;
		}

		// Old movement
		//transform.position = pos;
		rigbod.MovePosition (pos);

		// RESTRICT TO BOUNDS
		if (transform.position.y > boundY)
			pos.y = boundY;
		if (transform.position.y < (-1 * boundY))
			pos.y = (-1 * boundY);
		if (transform.position.x > boundX)
			pos.x = boundX;
		if (transform.position.x < (-1 * boundX))
			pos.x = (-1 * boundX);

		//transform.position = pos;
		rigbod.MovePosition (pos);

		// Shooting Animation Triggering
	
	}

	void Die() {
		Destroy (gameObject);
		Application.LoadLevel (0);
	}

	IEnumerator pauseForDeathAnimation() {
		yield return new WaitForSeconds(1);
		Die ();
	}
}

