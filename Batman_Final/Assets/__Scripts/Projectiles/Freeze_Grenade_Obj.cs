using UnityEngine;
using System.Collections;

public class Freeze_Grenade_Obj : MonoBehaviour
{
	private PE_Obj		batman;
	
	public bool			returning = false;
	public float		xSpeed = 15f;
	public float		ySpeed = 0f;
	public float		yAccel = -2f;

	void Start()
	{
		batman = GameObject.Find("Batman").GetComponent<PE_Obj>();
		if (batman.facing == PE_Facing.left)
			xSpeed *= -1f;
	}

	void FixedUpdate()
	{
		float dt = Time.fixedDeltaTime;
		Vector3 newPos = transform.position;
		newPos.x += xSpeed * dt;
		newPos.y += ySpeed * dt;

		transform.position = newPos;
		ySpeed += yAccel * dt;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.name == "Limiter" || other.tag == "Item")
			return;

		if (other.tag == "Enemy")
		{
			Enemy_Obj enemy = other.GetComponent<Enemy_Obj>();
			enemy.Freeze();
		}

		Destroy(this.gameObject);
	}

	void OnBecameInvisible()
	{
		Destroy (this.gameObject);
	}

}
