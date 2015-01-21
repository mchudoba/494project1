using UnityEngine;
using System.Collections;

public class Batman_Obj : MonoBehaviour
{

	private PE_Obj		thisPeo;

	public Vector3		vel; // Local velocity of Batman
	public GameObject	fist; // Reference to Batman's fist GameObject
	public float		minJumpVel = 5f;
	public float		maxJumpVel = 10f;
	public float		jumpRateIncrease = 0.1f;
	public float		h_speed = 6f; // Horizontal walking speed
	public float		duck = 0.66f; // Percentage to shrink Batman to duck
	public bool			atMaxJump = false;
	public bool			isJumping = false;
	public bool			grounded = false; // True if Batman is on the ground
	public bool			isDucked = false; // True if Batman is currently ducking
	public float		attackTimer = 0; // Timer for running an attack animation
	public float		attackTimerVal = 0.5f;
	public float		jumpTimer = 0;
	public float		jumpTimerVal = 0.1f;
	public float		xVelBeforeJump = 0;

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

		if (jumpTimer > 0)
		{
			jumpTimer -= Time.deltaTime;
			vel.x = 0;
		}

		Jump();
		Duck();
		Punch();

		thisPeo.vel = vel;
	}

	void Move()
	{
		float vX = Input.GetAxis("Horizontal");

		if (grounded || (vX * vel.x > 0 && xVelBeforeJump * vel.x >= 0)) vel.x = vX * h_speed;
		else if (vX * vel.x < 0 && Mathf.Abs(vel.x) > 1f) {
			if(vel.x > 0) vel.x -= 0.15f;
			else if (vel.x < 0) vel.x += 0.15f;
		}
		else if (Mathf.Abs(vel.x) > h_speed / 2f) vel.x /= 1.2f;
		else if (xVelBeforeJump == 0) vel.x = vX * h_speed / 3f;

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
		if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Period))
		    && grounded)
		{
			xVelBeforeJump = vel.x;
			vel.x = 0;
			jumpTimer = jumpTimerVal;
			isJumping = true;
			atMaxJump = true;

			Vector3 scale = transform.localScale;
			scale.y *= duck;
			transform.localScale = scale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			pos.y -= duck / 2f;
			transform.position = pos;

			return;
		}

		if (grounded && jumpTimer <= 0 && isJumping)
		{
			vel.x = xVelBeforeJump;
			vel.y = minJumpVel;
			thisPeo.ground = null; // Jumping will set ground = null
			atMaxJump = false;
			isJumping = false;

			Vector3 scale = transform.localScale;
			scale.y /= duck;
			transform.localScale = scale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			pos.y += duck / 2f;
			transform.position = pos;
		}

		if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Period))
		{
			if (vel.y < maxJumpVel && !atMaxJump)
			{
				vel.y += jumpRateIncrease;
			}
			else
			{
				atMaxJump = true;
			}
		}

		if (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.Period))
		{
			atMaxJump = true;

			if (jumpTimer <= 0)
				isJumping = false;
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