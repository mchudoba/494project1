using UnityEngine;
using System.Collections;

public class Enemy_Obj : MonoBehaviour
{
	private PE_Obj			thisPeo;
	private GameObject		batmanObj;
	private Animator		spriteAnimator;
	private PE_Facing		facingBeforeFrozen;
	private PE_Dir			dirBeforeFrozen;
	private Color			startColor;
	private float			velBeforeDamage;
	private float			closeEnough = 1f;
	private bool			takingDamage = false;
	private bool			killedByPlayer = false;
	private bool			despawnOnInvisible = true;
	private bool			wasFrozen = false;

	public GameObject		healthItem;
	public GameObject		ammoItem;
	public GameObject		sprite;
	public int				health = 0;
	public float			marchingSoldierSpeed = 7f;
	public float			spikeRobotSpeed1 = 4f;
	public float			spikeRobotSpeed2 = 7f;
	public float			damageTimer = 0;
	public float			damageTimerVal = 0.5f;
	public float			freezeTimer = 0;
	public float			freezeTimerVal = 5f;

	void Start()
	{
		if (Application.loadedLevelName == "_Custom_Level")
			despawnOnInvisible = false;

		thisPeo = GetComponent<PE_Obj>();
		batmanObj = GameObject.Find("Batman");
		startColor = sprite.renderer.material.color;
		spriteAnimator = sprite.GetComponent<Animator>();

		if (name.Contains("Marching Soldier"))
		{
			thisPeo.vel.x = marchingSoldierSpeed;
		}
		else if (name.Contains("Spike Robot"))
		{
			thisPeo.vel.x = spikeRobotSpeed1;
		}

		SetStartDirection();
	}

	void SetStartDirection()
	{
		if (wasFrozen)
		{
			thisPeo.facing = facingBeforeFrozen;
			thisPeo.dir = dirBeforeFrozen;
			wasFrozen = false;

			if (facingBeforeFrozen == PE_Facing.left)
				thisPeo.vel.x *= -1f;
			return;
		}

		float batmanX = batmanObj.transform.position.x;
		float thisX = transform.position.x;

		if (batmanX <= thisX) // Batman is left of enemy; face left
		{
			thisPeo.facing = PE_Facing.left;
			thisPeo.dir = PE_Dir.downLeft;
			thisPeo.vel.x *= -1f;
		}
		else // Batman is right of enemy; face right
		{
			thisPeo.facing = PE_Facing.right;
			thisPeo.dir = PE_Dir.downRight;
		}
	}

	void Update()
	{
		if (health <= 0)
			return;

		if (damageTimer > 0)
			damageTimer -= Time.deltaTime;
		else if (takingDamage == true)
		{
			takingDamage = false;
			sprite.renderer.material.color = startColor;
			spriteAnimator.enabled = true;
			thisPeo.vel.x = velBeforeDamage;
		}

		// Spike robot speed is faster if on the same level as Batman
		if (name.Contains("Spike Robot"))
		{
			float batmanBot = batmanObj.transform.position.y;
			float thisBot = transform.position.y;
			batmanBot -= batmanObj.transform.lossyScale.y / 2f;
			thisBot -= transform.lossyScale.y / 2f;

			// Batman and robot are on about the same level
			if (Mathf.Abs(batmanBot - thisBot) <= closeEnough)
			{
				if (thisPeo.vel.x > 0)
					thisPeo.vel.x = spikeRobotSpeed2;
				else if (thisPeo.vel.x < 0)
					thisPeo.vel.x = -spikeRobotSpeed2;
			}
			else
			{
				if (thisPeo.vel.x > 0)
					thisPeo.vel.x = spikeRobotSpeed1;
				else if (thisPeo.vel.x < 0)
					thisPeo.vel.x = -spikeRobotSpeed1;
			}
		}

		// Gunman and flamethrower always face the direction of Batman
		if (name.Contains("Flamethrower") || name.Contains("Gunman"))
		{
			float batmanX = batmanObj.transform.position.x;
			float thisX = transform.position.x;

			if (batmanX <= thisX)
				thisPeo.facing = PE_Facing.left;
			else
				thisPeo.facing = PE_Facing.right;
		}

		//Frozen
		if (freezeTimer > 0) {
			thisPeo.vel.x = 0;
			freezeTimer -= Time.deltaTime;
			if(sprite.renderer.material.color != Color.red)
				sprite.renderer.material.color = Color.cyan;
		} else if (sprite.renderer.material.color == Color.cyan)
		{
			spriteAnimator.enabled = true;
			sprite.renderer.material.color = startColor;
			Start();
		}

		// If enemy leaves frame of view, destroy it
		return;
	}

	public void TakeDamage(int damage)
	{
		spriteAnimator.enabled = false;
		takingDamage = true;
		velBeforeDamage = thisPeo.vel.x;
		thisPeo.vel.x = 0;
		sprite.renderer.material.color = Color.red;
		damageTimer = damageTimerVal;
		health -= damage;

		if (health <= 0)
			killedByPlayer = true;
	}

	public void Freeze()
	{
		spriteAnimator.enabled = false;
		thisPeo.vel.x = 0;
		facingBeforeFrozen = thisPeo.facing;
		dirBeforeFrozen = thisPeo.dir;
		wasFrozen = true;
		freezeTimer = freezeTimerVal;
		if (name.Contains("Flamethrower"))
		{
			GetComponent<Flamethrower_Obj>().Freeze(freezeTimerVal);
		}
		else if (name.Contains("Gunman"))
		{
			GetComponent<Gunman_Obj>().Freeze(freezeTimerVal);
		}
	}

	void OnBecameInvisible()
	{
		if (!despawnOnInvisible)
			return;

		if (name.Contains("Flamethrower") || name.Contains("Gunman"))
			return;

		if(freezeTimer <= 0){
			health = 0;
			renderer.enabled = false;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!name.Contains("Spike Robot") && !name.Contains("Marching Soldier"))
			return;
		if (other.name != "Limiter")
			return;

		thisPeo.vel.x *= -1f;
		if (thisPeo.facing == PE_Facing.left)
			thisPeo.facing = PE_Facing.right;
		else
			thisPeo.facing = PE_Facing.left;
	}

	void OnDestroy()
	{
		if (!killedByPlayer)
			return;

		int rand = Random.Range(1, 4);
		if (rand == 1)
		{
			Instantiate(ammoItem, new Vector3(transform.position.x, transform.position.y, -0.1f), Quaternion.identity);
		}
		else if (rand == 2)
		{
			Instantiate(healthItem, new Vector3(transform.position.x, transform.position.y, -0.1f), Quaternion.identity);
		}
		else
		{
			//points
		}
	}
}
