using UnityEngine;
using System.Collections;

public class OpenLevel : MonoBehaviour {
	public GameObject		controls;
	public GameObject		credits;

	// Update is called once per frame
	void Update () {
		Time.timeScale = 1f;
		if (Input.GetKey(KeyCode.Alpha1)) {
			Application.LoadLevel("_Classic_Level");
		}
		else if (Input.GetKey(KeyCode.Alpha2)) {
			Application.LoadLevel("_Custom_Level");
		}
		else if (Input.GetKey(KeyCode.Alpha3))
		{
			gameObject.SetActive(false);
			controls.SetActive(true);
		}
		else if (Input.GetKey(KeyCode.Alpha4))
		{
			gameObject.SetActive(false);
			credits.SetActive(true);
		}
	}
}
