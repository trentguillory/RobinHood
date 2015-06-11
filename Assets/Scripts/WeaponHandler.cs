using UnityEngine;
using System.Collections;

public class WeaponHandler : MonoBehaviour {
	BoxCollider2D boxcol;
	// Use this for initialization
	void Start () {
		boxcol = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Stab() {
		boxcol.offset = new Vector2 (.05f, .09f);
		StartCoroutine (LingerBeforeRetract ());
	}

	IEnumerator LingerBeforeRetract() {
		yield return new WaitForSeconds(0.5f);
		boxcol.offset = new Vector2 (.18f, -0.08f);
	}
}
