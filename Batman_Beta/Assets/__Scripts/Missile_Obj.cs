using UnityEngine;
using System.Collections;

public class Missile_Obj : MonoBehaviour
{
	public static int	count = 0;

	private PE_Obj		batman;
	
	public float		xSpeed = 13f;
	public int			damage = 10;

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
		transform.position = newPos;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			Enemy_Obj enemy = other.GetComponent<Enemy_Obj>();
			enemy.TakeDamage(damage);
		}

		if(other.tag != "Item"){
			count--;
			Destroy(this.gameObject);
		}
	}

	void OnBecameInvisible()
	{
		count--;
		Destroy(this.gameObject);
	}

}
