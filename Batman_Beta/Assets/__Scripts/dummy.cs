using UnityEngine;
using System.Collections;

public class dummy : MonoBehaviour
{
	public GameObject 	soldier;

	void OnBecameVisible()
	{
		GameObject newSoldier = Instantiate (soldier) as GameObject;
		newSoldier.transform.position = transform.position;
	}
}
