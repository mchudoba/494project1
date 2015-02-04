using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	public GameObject		batman;
	public float			minY = 4f;
	public float			minX = 2.9f;
	public float			maxX = 113f;
	public float			upperBound = 3f;
	public float			lowerBound = -2f;

	void Update()
	{
		float batman_x = batman.transform.position.x;
		float batman_y = batman.transform.position.y;
		float cam_x = batman_x;
		float cam_y = transform.position.y;

		PE_Obj batmanPeo = batman.GetComponent<PE_Obj>();
		if (batmanPeo.still)
		{
			cam_x = batman_x;
			cam_y = batman_y;
			transform.position = new Vector3(cam_x, cam_y, -10f);
			return;
		}

		float cam_y_delta = Mathf.Abs(batmanPeo.pos1.y - batmanPeo.pos0.y);
		float batman_delta = batman.transform.lossyScale.y / 2f;

		if (batman_y - cam_y + batman_delta > upperBound)
			cam_y += cam_y_delta;

		if (batman_y - cam_y - batman_delta < lowerBound)
			cam_y -= cam_y_delta;

		if (cam_y < minY)
			cam_y = minY;

		if (cam_x < minX)
			cam_x = minX;

		if (cam_x > maxX)
			cam_x = maxX;

		transform.position = new Vector3(cam_x, cam_y, -10f);
	}

}
