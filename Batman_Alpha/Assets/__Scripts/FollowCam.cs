using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	public GameObject		batman;

	void Update()
	{
		float batman_x = batman.transform.position.x;
		float batman_y = batman.transform.position.y;
		batman_y = this.transform.position.y; // TEMPORARY
		this.transform.position = new Vector3(batman_x, batman_y, -10f);
	}

}
