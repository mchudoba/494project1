using UnityEngine;
using System.Collections;

public class Flame_Obj : MonoBehaviour
{
	private Batman_Obj	batman;
	
	void Start()
	{
		batman = GameObject.FindGameObjectWithTag("Player").GetComponent<Batman_Obj>();
		renderer.enabled = false;
		collider.enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Player")
			return;

		batman.TakeDamage();
	}

	void OnTriggerStay(Collider other)
	{
		OnTriggerEnter(other);
	}

}
