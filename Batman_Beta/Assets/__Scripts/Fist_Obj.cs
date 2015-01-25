using UnityEngine;
using System.Collections;

public class Fist_Obj : MonoBehaviour
{
	public int			damage = 10;
	
	void Start ()
	{
		this.renderer.enabled = false;
		this.collider.enabled = false;
	}

}
