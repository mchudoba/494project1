using UnityEngine;
using System.Collections;

public class Gunman_Obj : MonoBehaviour
{
	private bool		ducked = false;
	private bool		fired = false;
	private float		duck = 0.66f;
	private Vector3		startScale;
	private PE_Obj		thisPeo;
	private Enemy_Obj	thisEnemy;

	public GameObject	bullet;
	public float		attackTimer = 0;
	public float		attackTimerVal = 2f;

	void Start()
	{
		attackTimer = 0;
		startScale = transform.localScale;
		thisPeo = GetComponent<PE_Obj>();
		thisEnemy = GetComponent<Enemy_Obj>();
	}

	void Update()
	{
		if (attackTimer > attackTimerVal)
		{
			attackTimer = 0;
			ducked = false;
			fired = false;
			Unduck();
		}
		else
			attackTimer += Time.deltaTime;

		if (attackTimer > 1.5f && fired == false)
		{
			Fire();
			fired = true;
		}
		if (attackTimer > 2f && ducked == false)
		{
			Duck();
			ducked = true;
		}
	}

	void Fire()
	{
		if (thisEnemy.health <= 0)
			return;

		Vector3 bulletPos = transform.position;
		if (thisPeo.facing == PE_Facing.right)
			bulletPos.x += 1f;
		else
			bulletPos.x -= 1f;
		
		bulletPos.y += transform.lossyScale.y / 4f;
		
		GameObject newBullet = Instantiate(bullet, bulletPos, Quaternion.identity) as GameObject;
		if (thisPeo.facing == PE_Facing.left)
		{
			newBullet.GetComponent<Bullet_Obj>().xSpeed *= -1f;
		}
	}

	void Duck()
	{
		Vector3 scale = transform.localScale;
		scale.y *= duck;
		transform.localScale = scale;
		
		// Move location back to ground
		Vector3 pos = transform.position;
		pos.y -= duck / 2f;
		transform.position = pos;
	}

	void Unduck()
	{
		transform.localScale = startScale;

		Vector3 pos = transform.position;
		pos.y += duck / 2f;
		transform.position = pos;
	}
}
