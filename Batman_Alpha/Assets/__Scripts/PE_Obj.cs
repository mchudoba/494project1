using UnityEngine;
using System.Collections;

// Direction characters are facing
public enum Direction
{
	Left,
	Right
}

public class PE_Obj : MonoBehaviour
{

	public Vector3		vel = Vector3.zero;
	public Vector3		pos0 = Vector3.zero;
	public Vector3		pos1 = Vector3.zero;
	public Direction	dir0 = Direction.Right;
	public Direction	dir1 = Direction.Right;

	void Start()
	{
		if (PhysEngine.objs.IndexOf(this) == -1)
			PhysEngine.objs.Add(this);
	}

	void OnTriggerEnter(Collider other)
	{
		PE_Obj otherObj = other.GetComponent<PE_Obj>();
		if (otherObj == null) return;
		
		ResolveCollisionWith(otherObj);
	}
	
//	void OnTriggerStay(Collider other)
//	{
//		OnTriggerEnter(other);
//	}
	
	public virtual void ResolveCollisionWith(PE_Obj other) {}

}
