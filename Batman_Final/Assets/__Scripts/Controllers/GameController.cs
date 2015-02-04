using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
	private static bool		levelEnd = false;
	private static float	whenPaused;
	private static Text		winText;

	public static void GameOver()
	{
		Application.LoadLevel(Application.loadedLevelName);
	}

	public static void LevelEnd()
	{
		levelEnd = true;
		winText.enabled = true;
		Time.timeScale = 0;
		whenPaused = Time.realtimeSinceStartup;
	}

	void Start()
	{
		winText = GameObject.Find("WinText").GetComponent<Text>();
		winText.enabled = false;
	}

	void Update()
	{
		if (!levelEnd)
			return;

		if (Time.realtimeSinceStartup - whenPaused > 1.5f)
		{
			levelEnd = false;
			winText.enabled = false;
			Application.LoadLevel("_Main_Menu");
		}
	}
}
