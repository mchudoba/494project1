using UnityEngine;
using System.Collections;

public class Door_Obj : MonoBehaviour
{
	public bool		opening = false;
	public bool		closing = false;
	public bool		frozen = false;
	private bool		vertical;
	private Vector3		startingPos;
	private Vector3		openPos;

	public float		speed = 20f;

	public void Open()
	{
		opening = true;
		closing = false;
	}

	public void Close()
	{
		closing = true;
		opening = false;
	}

	void Start()
	{
		if (transform.rotation.z == 0)
			vertical = true;
		else
			vertical = false;

		startingPos = transform.position;
		openPos = startingPos;

		if (vertical)
			openPos.y += transform.localScale.y;
		else
			openPos.x -= transform.localScale.x;
	}

	void FixedUpdate()
	{
		// Don't move door if it conflicts with something
		if (frozen)
			return;

		// Door opening animation
		if (opening)
		{
			if (transform.position == openPos)
				return;

			Vector3 newPos = transform.position;
			if (vertical)
			{
				newPos.y += speed * Time.fixedDeltaTime;
				if (newPos.y >= openPos.y)
					newPos = openPos;
			}
			else
			{
				newPos.x -= speed * Time.fixedDeltaTime;
				if (newPos.x <= openPos.x)
					newPos = openPos;
			}

			transform.position = newPos;
		}
		// Door closing animation
		else
		{
			if (transform.position == startingPos)
				return;

			Vector3 newPos = transform.position;
			if (vertical)
			{
				newPos.y -= speed * Time.fixedDeltaTime;
				if (newPos.y <= startingPos.y)
					newPos = startingPos;
			}
			else
			{
				newPos.x += speed * Time.fixedDeltaTime;
				if (newPos.x >= startingPos.x)
					newPos = startingPos;
			}

			transform.position = newPos;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (opening)
		{
			frozen = false;
			return;
		}

		if (other.tag == "Player" || other.tag == "Enemy")
		{
			if (closing)
				frozen = true;
		}
	}

	void OnTriggerStay(Collider other)
	{
		OnTriggerEnter(other);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			frozen = false;
		}
	}
}