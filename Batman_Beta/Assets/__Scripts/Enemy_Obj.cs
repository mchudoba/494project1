using UnityEngine;
using System.Collections;

public class Enemy_Obj : MonoBehaviour
{
	private PE_Obj			thisPeo;
	private GameObject		batmanObj;
	private Color			startColor;
	private float			velBeforeDamage;

	public int				health = 0;
	public float			closeEnough = 1f;
	public float			marchingSoldierSpeed = 7f;
	public float			spikeRobotSpeed1 = 4f;
	public float			spikeRobotSpeed2 = 7f;
	public float			damageTimer = 0;
	public float			damageTimerVal = 1f;

	void Start()
	{
		thisPeo = GetComponent<PE_Obj>();
		batmanObj = GameObject.Find("Batman");
		startColor = gameObject.renderer.material.color;

		if (name == "Marching Soldier")
		{
			thisPeo.vel.x = marchingSoldierSpeed;
		}
		else if (name == "Spike Robot")
		{
			thisPeo.vel.x = spikeRobotSpeed1;
		}

		SetStartDirection();
	}

	void SetStartDirection()
	{
		float batmanX = batmanObj.transform.position.x;
		float thisX = transform.position.x;

		if (batmanX <= thisX) // Batman is left of enemy; face left
		{
			thisPeo.dir = PE_Dir.downLeft;
			thisPeo.vel.x *= -1f;
		}
		else // Batman is right of enemy; face right
		{
			thisPeo.dir = PE_Dir.downRight;
		}
	}

	void Update()
	{
		if (damageTimer > 0)
			damageTimer -= Time.deltaTime;
		else
			gameObject.renderer.material.color = startColor;

		// Spike robot speed is faster if on the same level as Batman
		if (name == "Spike Robot")
		{
			float batmanBot = batmanObj.transform.position.y;
			float thisBot = transform.position.y;
			batmanBot -= batmanObj.transform.lossyScale.y / 2f;
			thisBot -= transform.lossyScale.y / 2f;

			// Batman and robot are on about the same level
			if (Mathf.Abs(batmanBot - thisBot) <= closeEnough)
			{
				if (thisPeo.vel.x >= 0)
					thisPeo.vel.x = spikeRobotSpeed2;
				else
					thisPeo.vel.x = -spikeRobotSpeed2;
			}
			else
			{
				if (thisPeo.vel.x >= 0)
					thisPeo.vel.x = spikeRobotSpeed1;
				else
					thisPeo.vel.x = -spikeRobotSpeed1;
			}
		}

		// Gunman and flamethrower always face the direction of Batman
		if (name == "Flamethrower" || name == "Gunman")
		{
			float batmanX = batmanObj.transform.position.x;
			float thisX = transform.position.x;

			if (batmanX <= thisX)
				thisPeo.dir = PE_Dir.downLeft;
			else
				thisPeo.dir = PE_Dir.downRight;
		}

		// If enemy leaves frame of view, destroy it
		return;
	}

	public void TakeDamage(int damage)
	{
		if (damageTimer <= 0)
		{
			velBeforeDamage = thisPeo.vel.x;
			thisPeo.vel.x = 0;
			gameObject.renderer.material.color = Color.red;
			health -= damage;
			damageTimer = damageTimerVal;
		}
	}

}
