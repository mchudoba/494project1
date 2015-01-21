using UnityEngine;
using System.Collections;

public class Batman_Obj : MonoBehaviour
{

	private PE_Obj		thisPeo;

	public Vector3		vel; // Local velocity of Batman
	public GameObject	fist; // Reference to Batman's fist GameObject
	public float		jumpVel = 10f;
	public float		h_speed = 6f; // Horizontal walking speed
	public float		duck = 0.66f; // Percentage to shrink Batman to duck
	public bool			grounded = false; // True if Batman is on the ground
	public bool			isDucked = false; // True if Batman is currently ducking
	public float		attackTimer; // Timer for running an attack animation
	public float		attackTimerVal = 0.5f;

	void Start ()
	{
		thisPeo = GetComponent<PE_Obj>();
	}
		
	void Update()
	{
		vel = thisPeo.vel;
		grounded = (thisPeo.ground != null);

		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;

		if (attackTimer <= 0)
			Move();

		Jump();
		Duck();
		Punch();

		thisPeo.vel = vel;
	}

	void Move()
	{
		float vX = Input.GetAxis("Horizontal");
		vel.x = vX * h_speed;

		// Change direction
		Quaternion rot = transform.rotation;
		if (vX > 0)
		{
			rot.y = 0;
			transform.rotation = rot;
		}
		else if (vX < 0)
		{
			rot.y = 180f;
			transform.rotation = rot;
		}
	}

	void Jump()
	{
		float vY = Input.GetAxis("Jump");
		if (grounded)
		{
			vel.y = vY * jumpVel;
			thisPeo.ground = null; // Jumping will set ground = null
		}
	}
	
	void Duck()
	{
		Vector3 scale = transform.localScale;
		
		if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		    && thisPeo.vel == Vector3.zero && !isDucked)
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
		     || thisPeo.vel != Vector3.zero) && isDucked)
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
			if (grounded)
				vel.x = 0;
		}
		else if (attackTimer <= 0)
		{
			fist.renderer.enabled = false;
			fist.collider.enabled = false;
		}
	}
}