using UnityEngine;
using System.Collections;

public class EnemyHandler : MonoBehaviour {

	public int health = 1;
	public enum CharacterName {Chicken, Knight, Archer};
	public CharacterName characterName;

	public enum CharacterType {Hostile, Scared};
	public CharacterType characterType;

	public float maxSpeed = 10f;
	public float rotSpeed = 90f;
	public float paceLength = 1;

	public float reloadDelay = 1f;
	float reloadTimer = 0;

	public enum PaceDir {Horizontal, Vertical};
	public PaceDir paceDir;
	public float viewDistance = 1;
	public bool inside = false;
	public bool WillPace = true;

	// upper left coord
	public Vector2 roomCoordinateUL = new Vector2 (0, 0);
	// lower right coord
	public Vector2 roomCoordinateLR = new Vector2 (1, 1);
	
	Vector3 spawnPoint = new Vector3 (0,0,0);

	// enemy states
	enum CharacterState {Pacing, Chasing, Returning, OnGuard};
	CharacterState characterState = CharacterState.Pacing;


	float speedForAnim = 0;
	bool attackForAnim = false;
	GameObject weapon;
	// variables for freezing on death
	bool isDead = false;
	Vector3 posOnDeath;
	Quaternion rotOnDeath;

	Animator anim;
	Rigidbody2D rigbod;
	Rigidbody2D rigbodPlayer;
	Transform player;

	// variables for initializing points for pacing
	Vector3 pacePoint1;
	//float pacePoint1Rot = 270;
	Vector3 pacePoint2;
	//float pacePoint2Rot = 90;
	bool hasSetPoints = false;
	bool atPoint1 = false;
	bool atPoint2 = false;

	float SaveSpeed;

	void Start () {
		anim = GetComponent<Animator> ();
		anim.SetFloat("health", (float)health);
		anim.SetFloat ("speed", speedForAnim);
		rigbod = GetComponent<Rigidbody2D> ();
		rigbodPlayer = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D> ();
		spawnPoint = transform.position;
		if (characterType == CharacterType.Hostile) {
			weapon = transform.FindChild("weapon").gameObject;
		}
		SaveSpeed = maxSpeed;

		if (!WillPace) {
			characterState = CharacterState.OnGuard;
		}
	}

	void OnTriggerEnter2D() {
		health--;
		float fhealth = (float)health;
		anim.SetFloat("health", fhealth);

		if (health <= 0) {
			pauseOnceKilled ();
			StartCoroutine(pauseForDeathAnimation());
		}
	}

	void Update() {
		reloadTimer -= Time.deltaTime;
		if (health >= 1) {
			if (characterState == CharacterState.Pacing) {
				Look ();
				Pace ();
			} else if (characterState == CharacterState.Chasing) {
				Look ();
				LookAtPlayer ();
				MoveForward ();
				if (CheckBoundaries (rigbod.position) == true && inside) {
					characterState = CharacterState.Returning;
				}
			} else if (characterState == CharacterState.Returning) {
				//Look ();
				ReturnToSpawn ();
			} else {
				anim.SetFloat ("speed", 0);
				Look ();
				LookAtPlayer ();
			}

			float distanceTemp = Vector2.Distance (rigbod.position, rigbodPlayer.position);
			//Debug.Log (distanceTemp);

			if (distanceTemp < .75 && characterType == CharacterType.Hostile) {
				if (reloadTimer <= 0) {
					anim.SetBool ("attacking", true);
					if (characterName == CharacterName.Knight) {
						weapon.GetComponent<WeaponHandler>().Stab ();
					}
					pauseForAttackAnimation();
				}	
				reloadTimer = reloadDelay;
			} else {
				anim.SetBool ("attacking", false);
			}
		}
	}
	
	void Die() {
		Destroy (gameObject);
	}

	void Look() {

		Ray ray = new Ray (transform.position, transform.up);
		RaycastHit hit;
		
		if (Physics.Raycast (ray, out hit, viewDistance)) {
			//viewDistance = hit.distance;
			if(hit.collider.tag == "Player" && characterState == CharacterState.Pacing) {
				characterState = CharacterState.Chasing;
			}
			/*
			if (hit.collider.tag == "Wall" && hit.distance < 0.2f) {
				Debug.Log ("Aviudubg wakks");
				AvoidWallsStart ();
			}
			*/
		}

		if (characterState == CharacterState.OnGuard && CheckBoundaries (rigbodPlayer.position) == false)
			characterState = CharacterState.Chasing;
		
		Debug.DrawRay (ray.origin, ray.direction * viewDistance, Color.red, 1);
	}

	void Pace() {
		//Debug.Log ("is pacing");
		// horizontal pacing
		if (paceDir == PaceDir.Horizontal) {
			float pacePoint1Rot = 270;
			float pacePoint2Rot = 90;
			if (hasSetPoints == false) {
				pacePoint1 = new Vector3 (paceLength + transform.position.x, transform.position.y, transform.position.z);
				pacePoint2 = new Vector3 (transform.position.x - paceLength, transform.position.y, transform.position.z);
				hasSetPoints = true;
			}
			if (isDead) {
				transform.position = posOnDeath;
				transform.rotation = rotOnDeath;
			}
		
			if (atPoint1 == false)
				LookAtPoint (pacePoint1);
		
			float objectRotation = transform.rotation.eulerAngles.z;
		
			if (Mathf.Abs ((objectRotation - pacePoint1Rot)) < .05 && transform.position.x < pacePoint1.x) 
				MoveForward ();
		
			if (transform.position.x > pacePoint1.x) {
				atPoint1 = true;
				atPoint2 = false;
			}
		
			if (atPoint1 == true) {
				LookAtPoint (pacePoint2);
			}
		
			if(Mathf.Abs ((objectRotation - pacePoint2Rot)) < .05 && transform.position.x > pacePoint2.x && atPoint1 == true) {
				//Debug.Log ("hi there, I'm moving");
				MoveForward ();
				Vector3 rotation = new Vector3 (0, 0, 0);
				transform.Rotate (rotation);
			}
		
			if (transform.position.x < pacePoint2.x) {
				atPoint2 = true;
				atPoint1 = false;
			}

		// vertical pacing
		} else {
			float pacePoint1Rot = 0;
			float pacePoint2Rot = 180;
			if (hasSetPoints == false) {
				pacePoint1 = new Vector3 (transform.position.x, paceLength + transform.position.y, transform.position.z);
				pacePoint2 = new Vector3 (transform.position.x, transform.position.y - paceLength, transform.position.z);
				hasSetPoints = true;
			}
			if (isDead) {
				transform.position = posOnDeath;
				transform.rotation = rotOnDeath;
			}

			if (atPoint1 == false)
				LookAtPoint (pacePoint1);
			
			float objectRotation = transform.rotation.eulerAngles.z;
			
			if (Mathf.Abs ((objectRotation - pacePoint1Rot)) < .05 && transform.position.y < pacePoint1.y) 
				MoveForward ();
			
			if (transform.position.y > pacePoint1.y) {
				atPoint1 = true;
				atPoint2 = false;
			}
			
			if (atPoint1 == true) {
				LookAtPoint (pacePoint2);
			}
			
			if (Mathf.Abs ((objectRotation - pacePoint2Rot)) < .05 && transform.position.y > pacePoint2.y && atPoint1 == true) {
				MoveForward ();
				Vector3 rotation = new Vector3 (0, 0, 0);
				transform.Rotate (rotation);
			}
			
			if (transform.position.y < pacePoint2.y) {
				atPoint2 = true;
				atPoint1 = false;
			}
		}
	}

	void MoveForward() {
		anim.SetFloat ("speed", 1);

		Vector3 pos = rigbod.position;
		
		Vector3 velocity = new Vector3 (0, maxSpeed * Time.deltaTime, 0);
		
		pos += transform.rotation * velocity;

		rigbod.MovePosition (pos);
	}

	// returns true if OUTSIDE of room
	bool CheckBoundaries(Vector2 gobject) {			
		if (gobject.x > roomCoordinateLR.x || gobject.x < roomCoordinateUL.x || gobject.y > roomCoordinateUL.y || gobject.y < roomCoordinateLR.y) {
			return true;	
		} else {
			return false;
		}
	}

	void ReturnToSpawn () {
		if (Mathf.Abs(transform.position.y - spawnPoint.y) > .2 || Mathf.Abs(transform.position.x - spawnPoint.x) > .2) {
			LookAtPoint (spawnPoint);
			MoveForward ();
		} else {
			characterState = CharacterState.OnGuard;
		}
	}

	void LookAtPlayer() {
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
		//if (reachedRoomBorder == false) {
			Vector3 dir = player.position - transform.position;
			dir.Normalize ();

			float zAngle;
			if (characterType == CharacterType.Hostile) {
				zAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg - 90;
			} else {
				zAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg - 270;
			}
		
			Quaternion desiredRotation = Quaternion.Euler (0, 0, zAngle);
		
			transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRotation, rotSpeed * Time.deltaTime);
		//}
	}

	void LookAtPoint(Vector3 point) {
		Vector3 dir = point - transform.position;
		dir.Normalize ();

		float zAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg - 90;
		Quaternion desiredRotation = Quaternion.Euler (0, 0, zAngle);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, desiredRotation, rotSpeed * Time.deltaTime);
	}
	/*
	void AvoidWallsStart() {
		public float maxSpeed = 0;
		StartCoroutine (pauseForRotationReset());
	}

	void AvoidWallsEnd() {
		public float maxSpeed = SaveSpeed;
	}

	IEnumerator pauseForRotationReset() {
		yield return new WaitForSeconds(1);
		AvoidWallsEnd ();
	}
	*/
	IEnumerator pauseForDeathAnimation() {
		yield return new WaitForSeconds(1);
		Die ();
	}
	IEnumerator pauseForAttackAnimation() {
		yield return new WaitForSeconds(1);
		Debug.Log ("Set To False");
		anim.SetBool ("attacking", false);
	}

	void pauseOnceKilled() {
		posOnDeath = transform.position;
		rotOnDeath = transform.rotation;
		isDead = true;
	}

}
