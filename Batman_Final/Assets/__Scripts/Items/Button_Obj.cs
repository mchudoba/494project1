﻿using UnityEngine;
using System.Collections;

public class Button_Obj : MonoBehaviour
{
	private Door_Obj		door;

	public GameObject		doorObj;
	public Color			activeColor = new Color(0, 1f, 0);
	public Color			inactiveColor = new Color(0, 0.31f, 0);

	void Start()
	{
		door = doorObj.GetComponent<Door_Obj>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			door.Open();
			gameObject.renderer.material.color = activeColor;
		}
	}

	void OnTriggerStay(Collider other)
	{
		OnTriggerEnter (other);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Enemy")
		{
			door.Close();
			gameObject.renderer.material.color = inactiveColor;
		}
	}
}
