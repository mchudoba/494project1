using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

	public static void GameOver()
	{
		print("GAME OVER");
		Application.LoadLevel("_Classic_Level");
	}

}
