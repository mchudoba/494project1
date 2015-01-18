using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysEngine : MonoBehaviour
{

	static public List<PE_Obj>	objs;

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

		// Resolve collisions
		foreach (PE_Obj current in objs)
		{
			ResolveCollisions(current);
		}

		// Finalize positions and rotations
		foreach (PE_Obj current in objs)
		{
			current.transform.position = current.pos1;
			ChangeDirection(current);
		}
	}

	public void TimeStep(PE_Obj current, float dt)
	{
		// Position
		current.pos1 = current.pos0 = current.transform.position;
		current.pos1 += current.vel * dt;
	}

	public void ResolveCollisions(PE_Obj current)
	{
		//
	}

	void ChangeDirection(PE_Obj current)
	{
		// If direction shouldn't change, return
		if (current.dir0 == current.dir1) return;

		if (current.dir0 == Direction.Left && current.dir1 == Direction.Right)
			current.transform.Rotate(0, 180f, 0);
		else if (current.dir0 == Direction.Right && current.dir1 == Direction.Left)
			current.transform.Rotate(0, 180f, 0);

		current.dir0 = current.dir1;
	}

}
