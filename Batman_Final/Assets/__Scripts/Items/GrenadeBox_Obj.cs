using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeBox_Obj : MonoBehaviour
{
	private Text		helpText;

	void Start()
	{
		helpText = GameObject.Find("GrenadeBoxText").GetComponent<Text>();
		helpText.enabled = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Batman_Obj batman = other.gameObject.GetComponent<Batman_Obj>();
			batman.weaponct = 5;
			batman.weapon = Weapon.freeze;
			helpText.enabled = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
			helpText.enabled = false;
	}
}