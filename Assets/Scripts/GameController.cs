using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	//OBJECT IS TO KILL THIS MANY KNIGHTS
	public int goal = 2;
	//List of Knights
	GameObject[] knights;

	GameObject uigo;
	Text uitext;

	// Use this for initialization
	void Start () {
		knights = GameObject.FindGameObjectsWithTag("Knight");
		uigo = GameObject.FindWithTag ("UI");
		Text uitext = uigo.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		int counter = 0;
		foreach (GameObject knight in knights) {
			if (knight == null)
				counter++;
		}
		if (counter == goal) {
			Debug.Log ("you win");
			uitext.text = "You win.";
		}

	}

	void Eliminate() {

	}
}
