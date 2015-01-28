using UnityEngine;
using System.Collections;

public class Fist_Obj : MonoBehaviour
{
	public int			damage = 10;
	
	void Start ()
	{
		this.renderer.enabled = false;
		this.collider.enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		Enemy_Obj enemy = other.GetComponent<Enemy_Obj>();
		if (enemy == null) return;

		enemy.TakeDamage(damage);
	}

}
