using UnityEngine;
using System.Collections;

public class Button_Obj : MonoBehaviour {

	public GameObject		door;
	public Color			activeColor = new Color(0, 1f, 0);
	public Color			inactiveColor = new Color(0, 0.31f, 0);

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy") {
			door.renderer.enabled = false;
			door.collider.enabled = false;
			gameObject.renderer.material.color = activeColor;
		}
	}

	void OnTriggerStay(Collider other){
		OnTriggerEnter (other);
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Player" || other.tag == "Enemy") {
			door.renderer.enabled = true;
			door.collider.enabled = true;
			gameObject.renderer.material.color = inactiveColor;
		}
	}
}
