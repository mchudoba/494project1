using UnityEngine;
using System.Collections;

public class SubMenu : MonoBehaviour
{
	public GameObject		mainMenu;

	void Update()
	{
		if (Input.GetKey (KeyCode.Escape))
		{
			gameObject.SetActive(false);
			mainMenu.SetActive(true);
		}
	}
}
