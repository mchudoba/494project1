using UnityEngine;
using System.Collections;

public enum Weapon
{
	fist,
	batarang,
	missile,
	shuriken,
	freeze
}

public class Batman_Obj : MonoBehaviour
{
	private PE_Obj			thisPeo;
	private Color			startColor;
	private float			xVelBeforeJump = 0;
	private GameObject		body;
	private Vector3			vel; // Local velocity of Batman
	private Vector3			startScale; // Local scale of Batman
	private Vector3			bodyStartScale;
	private Animator		animator;
	private bool			deadAnimation = false;
	private float			gameOverTimer = 1.5f;

	public int			health = 8;
	public int			ammo = 0;
	public int			weaponct = 4; //change to access later weapons like freeze grenade
	public Vector3		wallJumpVel = Vector3.zero;
	public GameObject	fist; // Reference to Batman's fist GameObject
	public GameObject	batarang;
	public GameObject 	missile;
	public GameObject	shuriken;
	public GameObject 	freeze;
	public Weapon		weapon = Weapon.fist;
	public float		minJumpVel = 5f;
	public float		maxJumpVel = 10f;
	public float		jumpRateIncrease = 0.1f;
	public float		h_speed = 6f; // Horizontal walking speed
	public float		duck = 0.66f; // Percentage to shrink Batman to duck
	public bool			GibsonMode = false; // GIBSON MODE: unlimited lives and ammo; invincible
	public bool			collidingWithWall = false;
	public bool			sliding = false;
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
	public float		knockbackVel = 8f;

	void Start ()
	{
		thisPeo = GetComponent<PE_Obj>();
		body = GameObject.Find("Body");
		startScale = transform.localScale;
		bodyStartScale = body.transform.localScale;
		startColor = body.renderer.material.color;
		animator = body.GetComponent<Animator>();
	}

	void FixedUpdate()
	{
		if (sliding)
		{
			Vector3 pos = transform.position;
			pos.x += 12f * Time.fixedDeltaTime;
			pos.y -= 12f * Time.fixedDeltaTime;
			transform.position = pos;
		}
	}
		
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.LoadLevel("_Main_Menu");

		if (sliding)
			return;

		// Toggles Gibson Mode
		// Allows unlimited health, unlimited ammo, and invincibility
		if (Input.GetKeyDown(KeyCode.G))
			GibsonMode = !GibsonMode;

		if (GibsonMode)
		{
			ammo = 99;
			health = 8;
		}

		if (health <= 0)
		{
			body.renderer.material.color = startColor;
			thisPeo.still = true;

			if (!deadAnimation)
			{
				animator.Play("Batman_Dead");
				deadAnimation = true;
			}

			gameOverTimer -= Time.deltaTime;

			if (gameOverTimer < 0)
				GameController.GameOver();
			return;
		}

		ChangeWeapon();

		float dt = Time.deltaTime;
		vel = thisPeo.vel;
		grounded = (thisPeo.ground != null);

		if (grounded)
		{
			animator.SetBool("BatmanFalling", false);
			animator.SetBool("BatmanGrounded", true);
			isWallJumping = false;
		}
		else
		{
			animator.SetBool("BatmanGrounded", false);
			if (vel.y < 0)
				animator.SetBool("BatmanFalling", true);
		}

		if (takeDamageTimer > 0)
			takeDamageTimer -= dt;
		else
		{
			body.renderer.material.color = startColor;
			fist.renderer.material.color = startColor;
		}

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

			// Can still register attacks while wall jumping
			if (weapon == Weapon.fist)
				Punch();
			else if (weapon == Weapon.batarang)
				Batarang();
			else if (weapon == Weapon.missile)
				Missile();
			else if (weapon == Weapon.shuriken)
				Shuriken();
			else if (weapon == Weapon.freeze)
				FGrenade();

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

		if (weapon == Weapon.fist)
			Punch();
		else if (weapon == Weapon.batarang)
			Batarang();
		else if (weapon == Weapon.missile)
			Missile();
		else if (weapon == Weapon.shuriken)
			Shuriken();
		else if (weapon == Weapon.freeze)
			FGrenade();

		thisPeo.vel = vel;
	}

	void ChangeWeapon()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (ammo == 0 && weaponct == 4)
				return;

			if (ammo == 0)
			{
				if (weapon == Weapon.fist)
					weapon = Weapon.freeze;
				else
					weapon = Weapon.fist;
				return;
			}

			weapon++;
			if ((int)weapon == weaponct) weapon = 0;
		}
	}

	void Move()
	{
		float vX = Input.GetAxis("Horizontal");
		if (grounded && vX != 0)
			animator.SetBool("BatmanWalking", true);
		else
			animator.SetBool("BatmanWalking", false);

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
			animator.SetBool("BatmanWall", true);
			//animator.SetBool("BatmanFalling", false);
			wallJumpTimerRunning = true;
			vel = Vector3.zero;
			vel.y = 0.75f;
		}
		else if (wallJumpTimerRunning)
		{
			animator.SetBool("BatmanFalling", false);
			animator.SetBool("BatmanWall", false);
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

			animator.SetBool("BatmanWalking", false);
			animator.SetBool("BatmanJumping", true);

			Vector3 spriteScale = body.transform.localScale;
			scale.y *= duck;
			spriteScale.y /= duck;
			transform.localScale = scale;
			body.transform.localScale = spriteScale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			Vector3 spritePos = body.transform.localPosition;
			pos.y -= duck / 2f;
			spritePos.y = 0.25f;
			transform.position = pos;
			body.transform.localPosition = spritePos;

			return;
		}

		// Batman jumps, setting an initial y velocity
		if (grounded && jumpTimer <= 0 && isJumping)
		{
			animator.SetBool("BatmanJumping", false);
			vel.x = xVelBeforeJump;
			vel.y = minJumpVel;
			thisPeo.ground = null; // Jumping will set ground = null
			atMaxJump = false;
			isJumping = false;

			transform.localScale = startScale;
			body.transform.localScale = bodyStartScale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			Vector3 spritePos = body.transform.localPosition;
			pos.y += duck / 2f;
			spritePos.y = 0;
			transform.position = pos;
			body.transform.localPosition = spritePos;
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
		Vector3 spriteScale = body.transform.localScale;
		
		if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		    && thisPeo.vel == Vector3.zero && !isDucked)
		{
			animator.SetBool("BatmanCrouching", true);
			isDucked = true;

			scale.y *= duck;
			spriteScale.y /= duck;
			transform.localScale = scale;
			body.transform.localScale = spriteScale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			Vector3 spritePos = body.transform.localPosition;
			pos.y -= duck / 2f;
			spritePos.y = 0.25f;
			transform.position = pos;
			body.transform.localPosition = spritePos;
		}
		if (((Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
		     || thisPeo.vel != Vector3.zero) && isDucked)
		{
			animator.SetBool("BatmanCrouching", false);
			isDucked = false;
			
			transform.localScale = startScale;
			body.transform.localScale = bodyStartScale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			Vector3	spritePos = body.transform.localPosition;
			pos.y += duck / 2f;
			spritePos.y = 0;
			transform.position = pos;
			body.transform.localPosition = spritePos;
		}
	}

	void Punch()
	{
		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Comma))
		{
			// Disables collider before re-enabling to allow for spam-punching
			// Otherwise, attackTimer would never reach 0 and spamming Punch would not deal any damage
			fist.collider.enabled = false;

			animator.SetBool("BatmanWalking", false);
			if (isDucked)
				animator.Play("Batman_Crouch_Punch", -1, 0f);
			else
				animator.Play("Batman_Punch", -1, 0f);
			fist.collider.enabled = true;
			attackTimer = attackTimerVal;
			if (grounded)
				vel.x = 0;
		}
		else if (attackTimer <= 0)
		{
			fist.collider.enabled = false;
		}
	}

	void Batarang()
	{
		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Comma))
		{
			if (Batarang_Obj.count >= 3)
				return;

			attackTimer = attackTimerVal;
			if (grounded)
				vel.x = 0;

			Vector3 batarangPos = transform.position;
			if (thisPeo.facing == PE_Facing.right)
				batarangPos.x += 1f;
			else
				batarangPos.x -= 1f;

			Instantiate(batarang, batarangPos, Quaternion.identity);
			Batarang_Obj.count++;

			ammo--;

			// Animate the throw
			animator.SetBool("BatmanWalking", false);
			if (isDucked)
				animator.Play ("Batman_Crouch_Throw", -1, 0f);
			else
				animator.Play("Batman_Throw", -1, 0f);
		}
	}

	void Missile()
	{
		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Comma))
		{
			if (Missile_Obj.count >= 3)
				return;
			
			attackTimer = attackTimerVal;
			if (grounded)
				vel.x = 0;
			
			Vector3 missilePos = transform.position;
			if (thisPeo.facing == PE_Facing.right)
				missilePos.x += 1f;
			else
				missilePos.x -= 1f;

			missilePos.y += transform.lossyScale.y / 4f;
			
			Instantiate(missile, missilePos, Quaternion.identity);
			Missile_Obj.count++;
			
			ammo -= 2;

			// Animate the shooting
			animator.SetBool("BatmanWalking", false);
			if (isDucked)
				animator.Play ("Batman_Crouch_Shoot", -1, 0f);
			else
				animator.Play ("Batman_Shoot", -1, 0f);
		}
	}

	void Shuriken()
	{
		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Comma))
		{
			if (Shuriken_Obj.count >= 1)
				return;
			
			attackTimer = attackTimerVal;
			if (grounded)
				vel.x = 0;
			
			Vector3 shurikenPos = transform.position;
			if (thisPeo.facing == PE_Facing.right)
				shurikenPos.x += 1f;
			else
				shurikenPos.x -= 1f;

			shurikenPos.y += transform.lossyScale.y / 4f;
			
			Instantiate(shuriken, shurikenPos, Quaternion.identity);
			Shuriken_Obj.count++;
			
			ammo -= 3;

			// Animate the shooting
			animator.SetBool("BatmanWalking", false);
			if (isDucked)
				animator.Play ("Batman_Crouch_Throw", -1, 0f);
			else
				animator.Play ("Batman_Throw", -1, 0f);
		}
	}

	void FGrenade()
	{
		if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Comma))
		{
			attackTimer = attackTimerVal;
			if (grounded)
				vel.x = 0;
			
			Vector3 gPos = transform.position;
			if (thisPeo.facing == PE_Facing.right)
				gPos.x += 1f;
			else
				gPos.x -= 1f;
			
			Instantiate(freeze, gPos, Quaternion.identity);

			// Animate the throw
			animator.SetBool("BatmanWalking", false);
			if (isDucked)
				animator.Play ("Batman_Crouch_Throw", -1, 0f);
			else
				animator.Play("Batman_Throw", -1, 0f);
		}
	}

	public void TakeDamage()
	{
		if (GibsonMode)
			return;

		if (takeDamageTimer <= 0)
		{
			if (grounded)
				thisPeo.vel.y = minJumpVel;

			body.renderer.material.color = Color.red;
			fist.renderer.material.color = Color.red;
			takeDamageTimer = takeDamageTimerVal;
			if (health > 0)
				health -= 1;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.name == "Exit_Trigger")
			GameController.LevelEnd();

		if (other.name == "IceSlope")
		{
			Vector3 scale = transform.localScale;
			Vector3 spriteScale = body.transform.localScale;

			animator.SetBool("BatmanSliding", true);
			isDucked = true;
			
			scale.y *= duck;
			spriteScale.y /= duck;
			transform.localScale = scale;
			body.transform.localScale = spriteScale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			Vector3 spritePos = body.transform.localPosition;
			pos.y -= duck / 2f;
			spritePos.y = 0.25f;
			transform.position = pos;
			body.transform.localPosition = spritePos;

			sliding = true;
			thisPeo.facing = PE_Facing.right;
			thisPeo.still = true;
			return;
		}

		if (other.tag != "Item")
			return;

		if (other.name.Contains("Health"))
		{
			if (health < 8)
				health++;
		}
		else if (other.name.Contains("Ammo"))
		{
			if (ammo + 10 <= 99)
				ammo += 10;
			else
				ammo = 99;
		}

		Destroy(other.gameObject);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.name == "IceSlope")
		{
			animator.SetBool("BatmanSliding", false);
			isDucked = false;
			
			transform.localScale = startScale;
			body.transform.localScale = bodyStartScale;
			
			// Move location back to ground
			Vector3 pos = transform.position;
			Vector3	spritePos = body.transform.localPosition;
			pos.y += duck / 2f;
			spritePos.y = 0;
			transform.position = pos;
			body.transform.localPosition = spritePos;

			sliding = false;
			thisPeo.still = false;
		}
	}
}
