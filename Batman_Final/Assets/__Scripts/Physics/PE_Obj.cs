using UnityEngine;
using System.Collections;

public class PE_Obj : MonoBehaviour
{
	public float		closeEnough = 0.5f;

	public bool			still = false;
	public Vector3		acc = Vector3.zero;
	public Vector3		vel = Vector3.zero;
	public Vector3		pos0 = Vector3.zero;
	public Vector3		pos1 = Vector3.zero;
	public PE_Dir		dir = PE_Dir.still;
	public PE_Facing	facing = PE_Facing.right;
	public PE_Obj		ground = null; // Stores whether this is on the ground

	void Start()
	{
		if (PhysEngine.objs.IndexOf(this) == -1)
			PhysEngine.objs.Add(this);
	}

	void Update()
	{
		if (tag == "Door" || tag == "Wall" || tag == "Ground")
			return;

		Quaternion rot = transform.rotation;
		if (facing == PE_Facing.right)
		{
			if (tag == "Player")
				rot.y = 0;
			else
				rot.y = 180f;
		}
		else
		{
			if (tag == "Player")
				rot.y = 180f;
			else
				rot.y = 0;
		}

		transform.rotation = rot;
	}

	void OnTriggerEnter(Collider other)
	{
		// Ignore collisions of still objects
		if (still) return;

		PE_Obj otherObj = other.GetComponent<PE_Obj>();
		if (otherObj == null) return;

		ResolveCollisionWith(otherObj);
	}
	
	void OnTriggerStay(Collider other)
	{
		OnTriggerEnter(other);
	}

	void OnTriggerExit(Collider other)
	{
		// Ignore collisions of still objects
		if (still) return;

		PE_Obj otherObj = other.GetComponent<PE_Obj>();
		if (otherObj == null) return;

		// This sets ground to null if the object falls off of the current ground
		// Jumping will also set ground to null
		if (ground == otherObj)
			ground = null;

		if (name == "Batman")
			GetComponent<Batman_Obj>().collidingWithWall = false;
	}
	
	void ResolveCollisionWith(PE_Obj that)
	{
		// If Batman and and enemy collide, do not do any position resolution
		// If 2 enemies collide, do not do any position resolution
		if (IgnoreCollision(that))
		{
			return;
		}

		// Assumes that "that" is still
		Vector3 posFinal = pos1; // Sets a default value for posFinal

		Vector3 a0, a1, b; // a0-moving corner last frame, a1-moving corner now, b-comparison corner on other object
		a0 = a1 = b = Vector3.zero;	 // Sets a default value to keep the compiler from complaining
		Vector3 delta = pos1 - pos0;
		
		if (dir == PE_Dir.down) {
			a1 = pos1;
			a1.y -= transform.lossyScale.y/2f;
			b = that.pos1;
			b.y += that.transform.lossyScale.y/2f;
			if (b.y - a1.y > 0 &&  b.y - a1.y <= closeEnough)
			{
				posFinal.y += Mathf.Abs( a1.y - b.y );
				vel.y = 0;

				if (ground == null) ground = that;
				transform.position = pos1 = posFinal;
			}
			else
			{
				if (name == "Batman" && that.tag != "Door")
					GetComponent<Batman_Obj>().collidingWithWall = true;
			}

			return;
		}
		
		if (dir == PE_Dir.up) {
			a1 = pos1;
			a1.y += transform.lossyScale.y/2f;
			b = that.pos1;
			b.y -= that.transform.lossyScale.y/2f;
			if (b.y - a1.y < 0 && b.y - a1.y >= -closeEnough)
			{
				posFinal.y -= Mathf.Abs( a1.y - b.y );
				vel.y = 0;
				transform.position = pos1 = posFinal;
			}
			else
			{
				if (name == "Batman" && that.tag != "Door")
					GetComponent<Batman_Obj>().collidingWithWall = true;
			}

			return;
		}
		
		if (dir == PE_Dir.upRight) { // Bottom, Left is the comparison corner
			a1 = pos1;
			a1.x += transform.lossyScale.x/2f;
			a1.y += transform.lossyScale.y/2f;
			a0 = a1 - delta;
			b = that.pos1;
			b.x -= that.transform.lossyScale.x/2f;
			b.y -= that.transform.localScale.y/2f;
		}
		
		if (dir == PE_Dir.upLeft) { // Bottom, Right is the comparison corner
			a1 = pos1;
			a1.x -= transform.lossyScale.x/2f;
			a1.y += transform.lossyScale.y/2f;
			a0 = a1 - delta;
			b = that.pos1;
			b.x += that.transform.lossyScale.x/2f;
			b.y -= that.transform.localScale.y/2f;
		}
		
		if (dir == PE_Dir.downLeft) { // Top, Right is the comparison corner
			a1 = pos1;
			a1.x -= transform.lossyScale.x/2f;
			a1.y -= transform.lossyScale.y/2f;
			a0 = a1 - delta;
			b = that.pos1;
			b.x += that.transform.lossyScale.x/2f;
			b.y += that.transform.localScale.y/2f;
		}
		
		if (dir == PE_Dir.downRight) { // Top, Left is the comparison corner
			a1 = pos1;
			a1.x += transform.lossyScale.x/2f;
			a1.y -= transform.lossyScale.y/2f;
			a0 = a1 - delta;
			b = that.pos1;
			b.x -= that.transform.lossyScale.x/2f;
			b.y += that.transform.localScale.y/2f;
		}
		
		// In the x dimension, find how far along the line segment between a0 and a1 we need to go to encounter b
		float u = (b.x - a0.x) / (a1.x - a0.x);
		
		// Determine this point using linear interpolation (see the appendix of the book)
		Vector3 pU = (1-u)*a0 + u*a1;
		
		// Use pU.y vs. b.y to tell which side of PE_Obj "that" PE_Obj "this" should be on
		switch (dir) {
		case PE_Dir.upRight:
			if (pU.y > b.y) { // hit the left side
				posFinal.x -= Mathf.Abs(a1.x - b.x);
				
				// Handle vel
				if (name == "Batman")
				{
					vel.x = 0;
					if (that.tag != "Door")
					{
						GetComponent<Batman_Obj>().collidingWithWall = true;
						GetComponent<Batman_Obj>().wallOnLeft = false;
					}
				}
				else if (tag == "Enemy")
				{
					vel.x *= -1f;
					facing = PE_Facing.left;
				}
				
			} else { // hit the bottom
				posFinal.y -= Mathf.Abs(a1.y - b.y);
				
				// Handle vel
				vel.y = 0;
				
			}
			break;
			
		case PE_Dir.downRight:
			if (pU.y < b.y) { // hit the left side
				posFinal.x -= Mathf.Abs(a1.x - b.x);
				
				// Handle vel
				if (name == "Batman")
				{
					vel.x = 0;
					if (that.tag != "Door")
					{
						GetComponent<Batman_Obj>().collidingWithWall = true;
						GetComponent<Batman_Obj>().wallOnLeft = false;
					}
				}
				else if (tag == "Enemy")
				{
					vel.x *= -1f;
					facing = PE_Facing.left;
				}
				
			} else { // hit the top
				posFinal.y += Mathf.Abs(a1.y - b.y);
				
				// Handle vel
				vel.y = 0;
				
				if (ground == null) ground = that;
			}
			break;
			
		case PE_Dir.upLeft:
			if (pU.y > b.y) { // hit the right side
				posFinal.x += Mathf.Abs(a1.x - b.x);
				
				// Handle vel
				if (name == "Batman")
				{
					vel.x = 0;
					if (that.tag != "Door")
					{
						GetComponent<Batman_Obj>().collidingWithWall = true;
						GetComponent<Batman_Obj>().wallOnLeft = true;
					}
				}
				else if (tag == "Enemy")
				{
					vel.x *= -1f;
					facing = PE_Facing.right;
				}
				
			} else { // hit the bottom
				posFinal.y -= Mathf.Abs(a1.y - b.y);
				
				// Handle vel
				vel.y = 0;
				
			}
			break;
			
		case PE_Dir.downLeft:
			if (pU.y < b.y) { // hit the right side
				posFinal.x += Mathf.Abs(a1.x - b.x);
				
				// Handle vel
				if (name == "Batman")
				{
					vel.x = 0;
					if (that.tag != "Door")
					{
						GetComponent<Batman_Obj>().collidingWithWall = true;
						GetComponent<Batman_Obj>().wallOnLeft = true;
					}
				}
				else if (tag == "Enemy")
				{
					vel.x *= -1f;
					facing = PE_Facing.right;
				};
				
			} else { // hit the top
				posFinal.y += Mathf.Abs(b.y - a1.y);
				
				// Handle vel
				vel.y = 0;
				
				if (ground == null) ground = that;
			}
			break;
		}

		this.transform.position = pos1 = posFinal;
	}

	bool IgnoreCollision(PE_Obj that)
	{
		if (name == "Batman" && that.tag == "Enemy")
		{
			if (that.GetComponent<Enemy_Obj>().health <= 0)
				return true;

			Batman_Obj batman = GetComponent<Batman_Obj>();
			batman.TakeDamage();
			if (batman.GibsonMode) // ignore knockback
				return true;

			if (facing == PE_Facing.left)
				vel.x = batman.knockbackVel;
			else
				vel.x = -batman.knockbackVel;

			return true;
		}
		else if (tag == "Enemy" && (that.name == "Batman" || that.tag == "Enemy"))
		{
			return true;
		}
		return false;
	}

}
