using UnityEngine;
using System.Collections;

public class Shuriken_Obj : MonoBehaviour
{
	public static int	count = 0;

	private bool		resized = false;
	private float		initialX = 0;
	private PE_Obj		batman;
	
	public float		xSpeed = 13f;
	public float		changeDistance = 1f;
	public int			damage = 10;

	void Start()
	{
		batman = GameObject.Find("Batman").GetComponent<PE_Obj>();
		if (batman.facing == PE_Facing.left)
			xSpeed *= -1f;

		initialX = transform.position.x;
	}

	void FixedUpdate()
	{
		if (Mathf.Abs(transform.position.x - initialX) >= changeDistance && !resized)
		{
			Vector3 newSize = transform.localScale;
			newSize.y *= 3;
			transform.localScale = newSize;
			resized = true;
		}

		float dt = Time.fixedDeltaTime;
		Vector3 newPos = transform.position;
		float newX = xSpeed * dt + transform.position.x;
		newPos.x = newX;

		transform.position = newPos;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy")
		{
			Enemy_Obj enemy = other.GetComponent<Enemy_Obj>();
			enemy.TakeDamage(damage);

			if (enemy.name.Contains("Gunman"))
				enemy.TakeDamage(damage);
		}
	}

	void OnBecameInvisible()
	{
		count--;
		Destroy(this.gameObject);
	}

}
