using UnityEngine;
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

public class PhysEngine : MonoBehaviour
{

	static public List<PE_Obj>	objs;

	public Vector3		gravity = new Vector3(0, -9.8f, 0);

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
