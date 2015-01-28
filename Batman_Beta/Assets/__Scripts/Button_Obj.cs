using UnityEngine;
using System.Collections;

public class Button_Obj : MonoBehaviour {

	public GameObject		door;
	public Color			active = new Color(0, 1f, 0);
	public Color			inactive = new Color(0, 0.31f, 0);

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy") {
			door.SetActive(false);
			gameObject.renderer.material.color = active;
		}
	}

	void OnTriggerStay(Collider other){
		OnTriggerEnter (other);
	}

	void OnTriggerExit(Collider other){
		if (other.tag == "Player" || other.tag == "Enemy") {
			door.SetActive(true);
			gameObject.renderer.material.color = inactive;
		}
	}
}
