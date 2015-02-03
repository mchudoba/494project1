using UnityEngine;
using System.Collections;

public class Bullet_Obj : MonoBehaviour
{	
	private Batman_Obj	batman;

	public float		xSpeed = 13f;

	void Start()
	{
		batman = GameObject.FindGameObjectWithTag("Player").GetComponent<Batman_Obj>();
	}
	
	void FixedUpdate()
	{
		float dt = Time.fixedDeltaTime;
		Vector3 newPos = transform.position;
		newPos.x += xSpeed * dt;
		transform.position = newPos;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			batman.TakeDamage();
		}
		
		if (other.tag != "Item" && other.name != "SpikeRobotLimiter")
		{
			Destroy(this.gameObject);
		}
	}
	
	void OnBecameInvisible()
	{
		Destroy(this.gameObject);
	}
	
}
