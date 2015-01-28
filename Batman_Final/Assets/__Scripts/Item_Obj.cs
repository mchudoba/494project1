using UnityEngine;
using System.Collections;

public class Item_Obj : MonoBehaviour
{
	public float		lifeTimer = 5f;

	void Start()
	{
		Destroy(this.gameObject, lifeTimer);
	}
}
