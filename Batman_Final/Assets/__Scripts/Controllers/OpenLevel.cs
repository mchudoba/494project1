using UnityEngine;
using System.Collections;

public class OpenLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Time.timeScale = 1f;
		if (Input.GetKey(KeyCode.Alpha1)) {
			Application.LoadLevel("_Classic_Level");
		}
		else if (Input.GetKey(KeyCode.Alpha2)) {
			Application.LoadLevel("_Custom_Level");
		}
	}
}
