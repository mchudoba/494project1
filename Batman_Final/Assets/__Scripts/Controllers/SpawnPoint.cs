using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
	private GameObject		enemy;
	private GameObject		batman;
	public float			respawnTimer = 0;
	private bool			cameraSpawn = true;
	private bool			spawnNow = false;

	public GameObject		enemyType;
	public float			respawnTimerVal = 2f;
	public float			spawnOffset = 1f;
	public Vector3			minPos = Vector3.zero;
	public Vector3			maxPos = Vector3.zero;
	public bool				limitX = false;
	public bool				limitY = false;
	public bool				limitZ = false;

	void Start()
	{
		GetComponent<TextMesh>().text = "";
		batman = GameObject.Find("Batman");

		// If the current level is the custom level, do not spawn enemies
		// based on the camera vision
		//if (Application.loadedLevelName == "_Custom_Level")
		//{
		//	cameraSpawn = false;
		//}
	}

	void OnBecameVisible()
	{
		if (enemy == null && respawnTimer <= 0)
		{
			respawnTimer = respawnTimerVal;
			enemy = Instantiate (enemyType) as GameObject;
			Vector3 pos = transform.position;

			if (enemyType.name != "Flamethrower" && enemyType.name != "Gunman")
			{
				if (batman.transform.position.x < pos.x)
					pos.x += spawnOffset;
				else
					pos.x -= spawnOffset;
			}

			// Enforce x, y, and z position limits
			if (limitX)
			{
				if (pos.x < minPos.x)
					pos.x = minPos.x;
				else if (pos.x > maxPos.x)
					pos.x = maxPos.x;
			}
			if (limitY)
			{
				if (pos.y < minPos.y)
					pos.y = minPos.y;
				else if (pos.y > maxPos.y)
					pos.y = maxPos.y;
			}
			if (limitZ)
			{
				if (pos.z < minPos.z)
					pos.z = minPos.z;
				else if (pos.z > maxPos.z)
					pos.z = maxPos.z;
			}

			enemy.transform.position = pos;
		}
	}

	void OnBecameInvisible()
	{
		if (respawnTimer <= 0)
			respawnTimer = respawnTimerVal;
	}

	void Update()
	{
		if (respawnTimer > 0)
			respawnTimer -= Time.deltaTime;
	}
}
