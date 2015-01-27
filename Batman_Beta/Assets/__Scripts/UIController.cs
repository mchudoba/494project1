using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
	private	Text		healthText;
	private Text		ammoText;
	private Batman_Obj	batman;
	private Color		purple = new Color(0.55f, 0, 1f);
	private Color		orange = new Color(1f, 0.57f, 0.17f);

	void Start()
	{
		healthText = GameObject.Find("HealthText").GetComponent<Text>();
		ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
		batman = GameObject.Find("Batman").GetComponent<Batman_Obj>();

		healthText.text = "HEALTH: " + batman.health;
		healthText.color = purple;
		ammoText.text = "BATMAN";
		ammoText.color = purple;
	}

	void Update()
	{
		healthText.text = "HEALTH: " + batman.health;

		if (batman.weapon == Weapon.fist)
			ammoText.text = "BATMAN";
		else if (batman.weapon == Weapon.batarang)
			ammoText.text = "BATARANG: " + batman.ammo;

		if (batman.ammo <= 0)
		{
			ammoText.color = purple;
			batman.weapon = Weapon.fist;
		}
		else
			ammoText.color = orange;
	}

}
