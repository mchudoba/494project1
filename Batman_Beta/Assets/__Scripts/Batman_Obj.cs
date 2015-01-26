using UnityEngine;
using System.Collections;

public class Batman_Obj : MonoBehaviour
{

	private PE_Obj		thisPeo;
	private Color		startColor;
	private GameObject	body;

	public int			health = 8;
	public Vector3		vel; // Local velocity of Batman
	public Vector3		startScale; // Local scale of Batman
	public Vector3		wallJumpVel = Vector3.zero;
	public GameObject	fist; // Reference to Batman's fist GameObject
	public float		minJumpVel = 5f;
	public float		maxJumpVel = 10f;
	public float		jumpRateIncrease = 0.1f;
	public float		h_speed = 6f; // Horizontal walking speed
	public float		duck = 0.66f; // Percentage to shrink Batman to duck
	public bool			collidingWithWall = false;
	public bool			wallOnLeft = true;
	public bool			isWallJumping = false;
	public bool			wallJumpTimerRunning = false;
	public bool			atMaxJump = false;
	public bool			isJumping = false;
	public bool			grounded = false; // True if Batman is on the ground
	public bool			isDucked = false; // True if Batman is currently ducking
	public float		attackTimer = 0; // Timer for running an attack animation
	public float		attackTimerVal = 0.5f;
	public float		jumpTimer = 0;
	public float		jumpTimerVal = 0.1f;
	public float		wallJumpTimer = 0;
	public float		wallJumpTimerVal = 0.2f;
	public float		takeDamageTimer = 0;
	public float		takeDamageTimerVal = 1f;
	public float		xVelBeforeJump = 0;
	public float		knockbackVel = 8f;

	void Start ()
	{
		thisPeo = GetComponent<PE_Obj>();
		body = GameObject.Find("Body");
		startScale = transform.localScale;
		startColor = body.renderer.material.color;
	}
		
	void Update()
	{
		if (health <= 0)
			GameController.GameOver();

		float dt = Time.deltaTime;
		vel = thisPeo.vel;
		grounded = (thisPeo.ground != null);

		if (grounded)
			isWallJumping = false;

		if (takeDamageTimer > 0)
			takeDamageTimer -= dt;
		else
			body.renderer.material.color = startColor;

		if (attackTimer > 0)
			attackTimer -= dt;

		if (wallJumpTimer > 0)
			wallJumpTimer -= dt;

		// Check for a wall jump only if Batman is colliding with a wall and not on the ground
		if (collidingWithWall && !grounded)
			WallJump();

		// If Batman is currently wall jumping, override all other controls
		if (isWallJumping)
		{
			thisPeo.vel = vel;
			return;
		}

		if (attackTimer <= 0)
			Move();

		if (jumpTimer > 0)
		{
			jumpTimer -= dt;
			vel.x = 0;
		}

		Jump(dt);
		Duck();
		Punch();

		thisPeo.vel = vel;
	}

	void Move()
	{
		float vX = Input.GetAxis("Horizontal");

		if (grounded || (vX * vel.x > 0 && xVelBeforeJump * vel.x >= 0)) vel.x = vX * h_speed;
		else if (vX * vel.x < 0 && Mathf.Abs(vel.x) > 1f)
		{
			if(vel.x > 0) vel.x -= 0.15f;
			else if (vel.x < 0) vel.x += 0.15f;
		}
		else if (Mathf.Abs(vel.x) > h_speed / 2f) vel.x /= 1.2f;
		else if (xVelBeforeJump == 0) vel.x = vX * h_speed / 3f;

		if (vX > 0)
			thisPeo.facing = PE_Facing.right;
		else if (vX < 0)
			thisPeo.facing = PE_Facing.left;
	}
	
	void WallJump()
	{
		if (wallJumpTimer > 0)
		{
			wallJumpTimerRunning = true;
			vel = Vector3.zero;
		}
		else if (wallJumpTimerRunning)
		{
			wallJumpTimerRunning = false;
			vel = wallJumpVel;
			if (thisPeo.facing == PE_Facing.right)
			{
				vel.x *= -1f;
				thisPeo.facing = PE_Facing.left;
			}
			else
				thisPeo.facing = PE_Facing.right;
		}

		if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Period)) && collidingWithWall)
		{
			if (!wallOnLeft && thisPeo.facing == PE_Facing.left)
				return;
			else if (wallOnLeft && thisPeo.facing == PE_Facing.right)
				return;
			else if (wallJumpTimer > 0)
				return;

			wallJumpTimer = wallJumpTimerVal;
			isWallJumping = true;
		}
	}

	void Jump(float dt)
	{
		// Start of jump; Batman crouches and stops moving in x direction
		if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Period))
		    && grounded && jumpTimer <= 0)
		{
			xVelBeforeJump = vel.x;
			vel.x = 0;
			jumpTimer = jumpTimerVal;
			isJumping = true;
			atMaxJump = true;

			Vector3 scale = transform.localScale;
			if (scale.y != startScale.y)
				return;

			scale.y *= duck;
			transform.localScale = scale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			pos.y -= duck / 2f;
			transform.position = pos;

			return;
		}

		// Batman jumps, setting an initial y velocity
		if (grounded && jumpTimer <= 0 && isJumping)
		{
			vel.x = xVelBeforeJump;
			vel.y = minJumpVel;
			thisPeo.ground = null; // Jumping will set ground = null
			atMaxJump = false;
			isJumping = false;

			transform.localScale = startScale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			pos.y += duck / 2f;
			transform.position = pos;
		}

		// Batman keeps jumping as long as jump button is held
		// There is a maximum y velocity that limits this
		if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.Period))
		{
			if (vel.y < maxJumpVel && !atMaxJump && vel.y != 0)
			{
				vel.y += jumpRateIncrease * dt;
			}
			else
			{
				atMaxJump = true;
			}
		}

		// No longer jumping
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
			
			transform.localScale = startScale;
			
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

	public void TakeDamage()
	{
		if (takeDamageTimer <= 0)
		{
			if (grounded)
				thisPeo.vel.y = minJumpVel;

			body.renderer.material.color = Color.red;
			takeDamageTimer = takeDamageTimerVal;
			if (health > 0)
				health -= 1;
		}
	}
}