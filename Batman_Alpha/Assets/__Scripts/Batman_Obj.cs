using UnityEngine;
using System.Collections;

public class Batman_Obj : PE_Obj
{

	public GameObject	fist; // Reference to Batman's fist GameObject
	public GameObject	frontCollider;
	public GameObject	backCollider;
	public GameObject	topCollider;
	public GameObject	bottomCollider;

	public float		h_speed = 6f; // Horizontal walking speed
	public float		duck = 0.66f; // Percentage to shrink Batman to duck
	public bool			isDucked = false; // True if Batman is currently ducking
	public float		attackTimer; // Timer for running an attack animation
	public float		attackTimerVal = 0.5f;

	void Update()
	{
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;

		if (attackTimer <= 0)
			Move();

		Duck();
		Punch();
	}

	void Move()
	{
		// Stop moving left
		if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
		{
			vel.x = 0;
		}
		// Stop moving right
		if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
		{
			vel.x = 0;
		}
		// Move left
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			vel.x = -h_speed;
			dir1 = Direction.Left;
		}
		// Move right
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
			vel.x = h_speed;
			dir1 = Direction.Right;
		}
	}
	
	void Duck()
	{
		Vector3 scale = transform.localScale;
		
		if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		    && vel == Vector3.zero && !isDucked)
		{
			isDucked = true;
			
			scale.y *= duck;
			transform.localScale = scale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			pos.y -= duck / 2f;
			transform.position = pos;
		}
		if (((Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
		     || vel != Vector3.zero) && isDucked)
		{
			isDucked = false;
			
			scale.y /= duck;
			transform.localScale = scale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			pos.y += duck / 2f;
			transform.position = pos;
		}
	}

	void Punch()
	{
		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Comma))
		{
			fist.renderer.enabled = true;
			fist.collider.enabled = true;
			attackTimer = attackTimerVal;
			vel.x = 0;
		}
		else if (attackTimer <= 0)
		{
			fist.renderer.enabled = false;
			fist.collider.enabled = false;
		}
	}

	public override void ResolveCollisionWith(PE_Obj other)
	{
		if (other.tag == "Floor")
		{
			vel.y = 0;
		}

		if (other.tag == "Wall")
		{
			vel.x = 0;
		}
	}
	
}