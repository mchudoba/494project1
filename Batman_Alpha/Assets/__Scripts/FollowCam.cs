using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	public GameObject		batman;
	public float			minY = 4f;

	void Update()
	{
		float batman_x = batman.transform.position.x;
		float batman_y = batman.transform.position.y;

		if (batman_y < minY)
			batman_y = minY;

		this.transform.position = new Vector3(batman_x, batman_y, -10f);
	}

}
