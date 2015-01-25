using UnityEngine;
using System.Collections;

public class Fist_Obj : MonoBehaviour
{
	public float			damage = 11f;
	
	void Start ()
	{
		this.renderer.enabled = false;
		this.collider.enabled = false;
	}

}
