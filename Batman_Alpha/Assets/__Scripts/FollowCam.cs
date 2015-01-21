using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	public GameObject		batman;
	public float			minY = 4f;
	public float			minX = 2.9f;

	void Update()
	{
		float batman_x = batman.transform.position.x;
		float batman_y = batman.transform.position.y;

		if (batman_y < minY)
			batman_y = minY;

		if (batman_x < minX)
			batman_x = minX;

		this.transform.position = new Vector3(batman_x, batman_y, -10f);
	}

}
