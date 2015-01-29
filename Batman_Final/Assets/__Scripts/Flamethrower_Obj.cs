using UnityEngine;
using System.Collections;

public class Flamethrower_Obj : MonoBehaviour
{
	private Enemy_Obj	thisEnemy;

	public GameObject	flame;
	public float		flameTimer = 0;
	public float		flameTimerVal = 1f;

	void Start()
	{
		thisEnemy = GetComponent<Enemy_Obj>();
	}

	void Update()
	{
		if (thisEnemy.health <= 0)
		{
			flame.renderer.enabled = false;
			flame.collider.enabled = false;
			return;
		}

		flameTimer -= Time.deltaTime;
		if (flameTimer < 0)
		{
			flameTimer = flameTimerVal;
			flame.renderer.enabled = !flame.renderer.enabled;
			flame.collider.enabled = !flame.collider.enabled;
		}
	}

}
