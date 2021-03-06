﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PE_Dir // The direction in which the PE_Obj is moving
{
	still,
	up,
	down,
	upRight,
	downRight,
	downLeft,
	upLeft
}

public enum PE_Facing // The direction in which the PE_Obj is facing
{
	left,
	right
}

public class PhysEngine : MonoBehaviour
{

	static public List<PE_Obj>	objs;

	public Vector3		gravity = new Vector3(0, -9.8f, 0);
	public float		terminalVel = -15f;
	public float		killTime = 0.5f;

	void Awake()
	{
		objs = new List<PE_Obj>();
	}

	void FixedUpdate()
	{
		// Handle the timestep for each object
		float dt = Time.fixedDeltaTime;
		foreach (PE_Obj current in objs)
			TimeStep(current, dt);

		// Finalize positions and rotations
		foreach (PE_Obj current in objs)
		{
			current.transform.position = current.pos1;
		}

		// Destroy any dead objects
		for (int i = objs.Count - 1; i >= 0; i--)
		{
			// Destroy enemies with <=0 health
			if (objs[i].tag == "Enemy")
			{
				Enemy_Obj kill = objs[i].GetComponent<Enemy_Obj>();
				if (kill.health <= 0)
				{
					kill.gameObject.renderer.material.color = Color.red;
					Destroy(kill.gameObject, killTime);
					objs.RemoveAt(i);
				}
			}
		}
	}

	public void TimeStep(PE_Obj current, float dt)
	{
		if (current.still)
		{
			current.pos0 = current.pos1 = current.transform.position;
			return;
		}

		// Velocity
		Vector3 tAcc = current.acc;
		tAcc += gravity;

		if (current.vel.y + (tAcc.y * dt) > terminalVel)
			current.vel += tAcc * dt;

		// Determine direction of current object
		if (current.vel.x == 0)
		{
			if (current.vel.y > 0)
				current.dir = PE_Dir.up;
			else
				current.dir = PE_Dir.down;
		}
		else if (current.vel.x>0 && current.vel.y>0)
		{
			current.dir = PE_Dir.upRight;
		}
		else if (current.vel.x>0 && current.vel.y<=0)
		{
			current.dir = PE_Dir.downRight;
		}
		else if (current.vel.x<0 && current.vel.y<=0)
		{
			current.dir = PE_Dir.downLeft;
		}
		else if (current.vel.x<0 && current.vel.y>0)
		{
			current.dir = PE_Dir.upLeft;
		}

		// Position
		current.pos1 = current.pos0 = current.transform.position;
		current.pos1 += current.vel * dt;
	}

}
