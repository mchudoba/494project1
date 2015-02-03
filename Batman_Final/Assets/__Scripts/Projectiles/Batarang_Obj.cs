using UnityEngine;
using System.Collections;

public class Batarang_Obj : MonoBehaviour
{
	public static int	count = 0;

	private bool		flipped = false;
	private float		initialX = 0;
	private float		closeEnough = 0.1f;
	private PE_Obj		batman;
	
	public bool			returning = false;
	public float		xSpeed = 15f;
	public float		ySpeed = 10f;
	public float		maxDistance = 3f;
	public int			damage = 5;

	void Start()
	{
		batman = GameObject.Find("Batman").GetComponent<PE_Obj>();
		if (batman.facing == PE_Facing.left)
			xSpeed *= -1f;

		initialX = transform.position.x;
	}

	void FixedUpdate()
	{
		if (Mathf.Abs(transform.position.x - initialX) >= maxDistance)
		{
			returning = true;
			initialX = batman.transform.position.x;
			if (!flipped)
			{
				xSpeed *= -1f;
				flipped = true;
			}
		}
		else
		{
			flipped = false;
		}

		float dt = Time.fixedDeltaTime;
		Vector3 newPos = transform.position;
		float newX = xSpeed * dt + transform.position.x;
		newPos.x = newX;

		if (returning)
		{
			float batmanY = batman.transform.position.y + (batman.transform.lossyScale.y / 4f);
			float newY = newPos.y;
			if (newY - batmanY > closeEnough)
			{
				newY -= ySpeed * dt;
			}
			else if (batmanY - newY > closeEnough)
			{
				newY += ySpeed * dt;
			}

			newPos.y = newY;
		}

		transform.position = newPos;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.name == "Batman")
		{
			Destroy(this.gameObject);
			count--;
		}
		else if (other.tag == "Enemy")
		{
			Enemy_Obj enemy = other.GetComponent<Enemy_Obj>();
			enemy.TakeDamage(damage);

			if (enemy.name.Contains("Gunman"))
				enemy.TakeDamage(damage);
		}
	}

}
