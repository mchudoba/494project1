using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
	private	Text		healthText;
	private Batman_Obj	batman;

	void Start()
	{
		healthText = GameObject.Find("HealthText").GetComponent<Text>();
		batman = GameObject.Find("Batman").GetComponent<Batman_Obj>();

		healthText.text = "HEALTH: " + batman.health;
	}

	void Update()
	{
		healthText.text = "HEALTH: " + batman.health;
	}

}
